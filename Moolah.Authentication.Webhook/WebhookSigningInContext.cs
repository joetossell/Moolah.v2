using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Moolah.Authentication.Webhook
{
    public class WebhookSigningInContext : PrincipalContext<WebhookAuthenticationOptions>
    {
        /// <summary>
        /// Creates a new instance of the context object.
        /// </summary>
        /// <param name="context">The HTTP request context</param>
        /// <param name="scheme">The scheme data</param>
        /// <param name="options">The handler options</param>
        /// <param name="principal">Initializes Principal property</param>
        /// <param name="properties">The authentication properties.</param>
        /// <param name="cookieOptions">Initializes options for the authentication cookie.</param>
        public WebhookSigningInContext(
            HttpContext context,
            AuthenticationScheme scheme,
            WebhookAuthenticationOptions options,
            ClaimsPrincipal principal,
            AuthenticationProperties properties)
            : base(context, scheme, options, properties)
        {
            Principal = principal;
        }
    }
}
