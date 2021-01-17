using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Moolah.Authentication.Webhook
{
    public class PostConfigureWebhookAuthenticationOptions : IPostConfigureOptions<WebhookAuthenticationOptions>
    {
        private readonly IDataProtectionProvider _dp;
        private readonly Func<IWebhookManager> _webhookManager;

        public PostConfigureWebhookAuthenticationOptions(IDataProtectionProvider dataProtection, Func<IWebhookManager> webhookManager)
        {
            _dp = dataProtection;
            _webhookManager = webhookManager;
        }

        /// <summary>
        /// Invoked to post configure a TOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, WebhookAuthenticationOptions options)
        {
            options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;

            if (options.TicketDataFormat == null)
            {
                // Note: the purpose for the data protector must remain fixed for interop to work.
                var dataProtector = options.DataProtectionProvider.CreateProtector("Moolah.Authentication.Webhook.WebhookAuthenticationMiddleware", name, "v2");
                options.TicketDataFormat = new TicketDataFormat(dataProtector);
            }
            if (options.WebhookManagerFactory == null)
            {
                options.WebhookManagerFactory = _webhookManager;
            }
        }
    }
}
