using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace Moolah.Authentication.Webhook
{
    public class WebhookAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Create an instance of the options initialized with the default values
        /// </summary>
        public WebhookAuthenticationOptions()
        {
            ExpireTimeSpan = TimeSpan.FromDays(14);
            SlidingExpiration = true;
            Events = new WebhookAuthenticationEvents();
        }

        /// <summary>
        /// If set this will be used by the CookieAuthenticationHandler for data protection.
        /// </summary>
        public IDataProtectionProvider DataProtectionProvider { get; set; }

        /// <summary>
        /// The SlidingExpiration is set to true to instruct the handler to re-issue a new token with a new
        /// expiration time any time it processes a request which is more than halfway through the expiration window.
        /// </summary>
        public bool SlidingExpiration { get; set; }

        /// <summary>
        /// The Provider may be assigned to an instance of an object created by the application at startup time. The handler
        /// calls methods on the provider which give the application control at certain points where processing is occurring.
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        public new WebhookAuthenticationEvents Events
        {
            get => (WebhookAuthenticationEvents)base.Events;
            set => base.Events = value;
        }

        /// <summary>
        /// The TicketDataFormat is used to protect and unprotect the identity and other properties which are stored in the
        /// query string value. If not provided one will be created using <see cref="DataProtectionProvider"/>.
        /// </summary>
        public ISecureDataFormat<AuthenticationTicket> TicketDataFormat { get; set; }

        /// <summary>
        /// The component used to get cookies from the request or set them on the response.
        ///
        /// ChunkingCookieManager will be used by default.
        /// </summary>
        public Func<IWebhookManager> WebhookManagerFactory { get; set; }

        /// <summary>
        /// <para>
        /// Controls how much time the authentication ticket stored in the query string will remain valid from the point it is created
        /// The expiration information is stored in the protected query string ticket. Because of that an expired query string will be ignored
        /// even if it is passed to the server after the browser should have purged it.
        /// </para>
        /// </summary>
        public TimeSpan ExpireTimeSpan { get; set; }
    }
}
