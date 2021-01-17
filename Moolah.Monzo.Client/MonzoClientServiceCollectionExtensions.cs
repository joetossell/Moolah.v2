using System;
using Moolah.Monzo.Client;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class MonzoClientServiceCollectionExtensions
    {
        public static void AddMonzoClient(this IServiceCollection services, string baseUrl)
        {
            var url = new Uri(baseUrl);
            services.AddHttpClient<WhoAmIClient>(client =>
                {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .AddUserAccessTokenHandler();

            services.AddHttpClient<AccountsClient>(client =>
                {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .AddUserAccessTokenHandler();

            services.AddHttpClient<PotClient>(client =>
                {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .AddUserAccessTokenHandler();

            services.AddHttpClient<TransactionClient>(client =>
                {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .AddUserAccessTokenHandler();

            services.AddHttpClient<WebhookClient>(client =>
                {
                    client.BaseAddress = url;
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .AddUserAccessTokenHandler();
        }
    }
}
