using System;
using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moolah.Monzo.IdentityModel;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for IServiceCollection to register the token management services
    /// </summary>
    public static class IdentityTokenManagementServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the token management services to DI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TokenManagementBuilder AddIdentityAccessTokenManagement(this IServiceCollection services, Action<AccessTokenManagementOptions> options = null)
        {
            if (options != null)
            {
                services.Configure(options);
            }

            services.AddHttpContextAccessor();
            services.AddAuthentication();

#if NETCOREAPP3_1
            services.AddDistributedMemoryCache();
#endif

            services.TryAddTransient<IAccessTokenManagementService, AccessTokenManagementService>();
            services.TryAddTransient<ITokenClientConfigurationService, OAuthTokenClientConfigurationService>();
            services.TryAddTransient<ITokenEndpointService, TokenEndpointService>();

            services.AddHttpClient(AccessTokenManagementDefaults.BackChannelHttpClientName);

            services.AddTransient<UserAccessTokenHandler>();
            services.AddTransient<ClientAccessTokenHandler>();

            services.TryAddTransient<IUserTokenStore, IdentityUserTokenStore>();
            services.TryAddTransient<IClientAccessTokenCache, ClientAccessTokenCache>();

            return new TokenManagementBuilder(services);
        }

    }
}
