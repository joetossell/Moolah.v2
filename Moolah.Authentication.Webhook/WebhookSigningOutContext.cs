using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Moolah.Authentication.Webhook
{
    public class WebhookSigningOutContext : PropertiesContext<WebhookAuthenticationOptions>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <param name="options"></param>
        /// <param name="properties"></param>
        /// <param name="cookieOptions"></param>
        public WebhookSigningOutContext(
            HttpContext context,
            AuthenticationScheme scheme,
            WebhookAuthenticationOptions options,
            AuthenticationProperties properties)
            : base(context, scheme, options, properties) { }
    }
}
