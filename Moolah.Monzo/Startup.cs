using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moolah.Monzo.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moolah.Monzo.Client.Serialization;
using Moolah.Monzo.IdentityModel;
using Moolah.Monzo.Middleware;
using Moolah.Monzo.Services;
using Polly;

namespace Moolah.Monzo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddRazorPages();
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new EmptyDateTimeOffsetToNullConverter());
                });

            // Note to self use identity server and store external tokens using UpdateExternalAuthenticationTokensAsync
            // Access tokens to be retrieved as Simple delegation using an extension grant
            // Azure function microservices architecture with Blazor frontend?
            // Note identity server no longer open source???
            services.AddAuthentication()
                .AddOAuth<ExtendedOAuthOptions, OAuthHandler<ExtendedOAuthOptions>>("Monzo", "Monzo", options =>
                {
                    options.ClientId = Configuration["Monzo:ClientId"];
                    options.ClientSecret = Configuration["Monzo:ClientSecret"];
                    options.CallbackPath = new PathString("/signin-monzo");
                    options.ClaimActions.MapUniqueJsonKey(ClaimTypes.NameIdentifier, "user_id");
                    options.SaveTokens = true;

                    options.Events.OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        var response = await context.Backchannel.SendAsync(request,
                            HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();
                        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                        context.RunClaimActions(json.RootElement);
                    };

                    options.AuthorizationEndpoint = "https://auth.monzo.com/";
                    options.TokenEndpoint = "https://api.monzo.com/oauth2/token";
                    options.UserInformationEndpoint = "https://api.monzo.com/ping/whoami";
                    options.RevocationEndpoint = "https://api.monzo.com/oauth2/logout";
                })
                .AddWebhook<WebHookService>();

            // adds user and client access token management
            services.AddIdentityAccessTokenManagement(options =>
                {
                    options.User = new AccessTokenManagementOptions.UserOptions()
                    {
                        Scheme = "Monzo",
                        RefreshBeforeExpiration = TimeSpan.FromHours(1)
                    };
                })
                .ConfigureBackchannelHttpClient()
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            services.AddMonzoClient(Configuration["Monzo:BaseUrl"]);
            services.AddTransient<EmailService>();
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRequestResponseLogging();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
