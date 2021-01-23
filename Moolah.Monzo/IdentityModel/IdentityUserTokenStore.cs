using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Moolah.Monzo.IdentityModel
{
    public class IdentityUserTokenStore : IUserTokenStore
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthorizationPolicyProvider _policyProvider;
        private readonly AccessTokenManagementOptions _options;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="contextAccessor"></param>
        /// <param name="userManager"></param>
        /// <param name="options"></param>
        public IdentityUserTokenStore(
            IHttpContextAccessor contextAccessor,
            UserManager<IdentityUser> userManager,
            IOptions<AccessTokenManagementOptions> options,
            IAuthorizationPolicyProvider policyProvider)
        {
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider)); ;
            _options = options.Value;
        }

        /// <inheritdoc/>
        public async Task<UserAccessToken> GetTokenAsync(ClaimsPrincipal user)
        {
            var identityUser = await _userManager.GetUserAsync(user);
            var accessToken = await _userManager.GetAuthenticationTokenAsync(identityUser, _options.User.Scheme,
                OpenIdConnectParameterNames.AccessToken);
            if (accessToken == null)
            {
                throw new InvalidOperationException("No access token found in database. An access token must be requested and UpdateExternalAuthenticationTokensAsync must be called in ExternalLogin callback.");
            }

            var refreshToken = await _userManager.GetAuthenticationTokenAsync(identityUser, _options.User.Scheme, OpenIdConnectParameterNames.RefreshToken);

            var expiresAt = await _userManager.GetAuthenticationTokenAsync(identityUser, _options.User.Scheme, "expires_at");
            if (expiresAt == null)
            {
                throw new InvalidOperationException("No expires_at value found in database.");
            }

            var dtExpires = DateTimeOffset.Parse(expiresAt, CultureInfo.InvariantCulture);

            var token = new UserAccessToken
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = dtExpires
            };

            return token;
        }

        /// <inheritdoc/>
        public async Task StoreTokenAsync(ClaimsPrincipal user, string accessToken, DateTimeOffset expiration, string refreshToken)
        {
            await StoreTokenInDatabaseAsync(user, accessToken, expiration, refreshToken);
        }

        private async Task StoreTokenInDatabaseAsync(ClaimsPrincipal user, string accessToken, DateTimeOffset expiration, string refreshToken)
        {
            var iu = await _userManager.GetUserAsync(user);
            await _userManager.SetAuthenticationTokenAsync(iu, _options.User.Scheme,
                OpenIdConnectParameterNames.AccessToken, accessToken);
            if (refreshToken != null)
            {
                await _userManager.SetAuthenticationTokenAsync(iu, _options.User.Scheme,
                    OpenIdConnectParameterNames.RefreshToken, refreshToken);
            }
            await _userManager.SetAuthenticationTokenAsync(iu, _options.User.Scheme, "expires_at", expiration.ToString("o", CultureInfo.InvariantCulture));
        }

        /// <inheritdoc/>
        public Task ClearTokenAsync(ClaimsPrincipal user)
        {
            return Task.CompletedTask;
        }

    }
}
