using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Moolah.Authentication.Webhook
{
    public class WebhookAuthenticationHandler : SignInAuthenticationHandler<WebhookAuthenticationOptions>
    {
        private bool _shouldRefresh;
        private bool _signInCalled;
        private bool _signOutCalled;

        private DateTimeOffset? _refreshIssuedUtc;
        private DateTimeOffset? _refreshExpiresUtc;
        private Task<AuthenticateResult> _readWebhookTask;
        private AuthenticationTicket _refreshTicket;
        private IWebhookManager _webhookManager;
        
        public WebhookAuthenticationHandler(IOptionsMonitor<WebhookAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring.
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new WebhookAuthenticationEvents Events
        {
            get { return (WebhookAuthenticationEvents)base.Events; }
            set { base.Events = value; }
        }

        protected override Task InitializeHandlerAsync()
        {
            _webhookManager = Options.WebhookManagerFactory();
            // Refresh the webhook once the response has been sent back
            Context.Response.OnCompleted(Finish);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a new instance of the events instance.
        /// </summary>
        /// <returns>A new instance of the events instance.</returns>
        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new WebhookAuthenticationEvents());

        private Task<AuthenticateResult> EnsureWebhookTicket()
        {
            // We only need to read the ticket once
            if (_readWebhookTask == null)
            {
                _readWebhookTask = ReadWebhookTicket();
            }
            return _readWebhookTask;
        }

        private void CheckForRefresh(AuthenticationTicket ticket)
        {
            var currentUtc = Clock.UtcNow;
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;
            var allowRefresh = ticket.Properties.AllowRefresh ?? true;
            if (issuedUtc != null && expiresUtc != null && Options.SlidingExpiration && allowRefresh)
            {
                var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                var timeRemaining = expiresUtc.Value.Subtract(currentUtc);

                if (timeRemaining < timeElapsed)
                {
                    RequestRefresh(ticket);
                }
            }
        }

        private void RequestRefresh(AuthenticationTicket ticket, ClaimsPrincipal replacedPrincipal = null)
        {
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (issuedUtc != null && expiresUtc != null)
            {
                _shouldRefresh = true;
                var currentUtc = Clock.UtcNow;
                _refreshIssuedUtc = currentUtc;
                var timeSpan = expiresUtc.Value.Subtract(issuedUtc.Value);
                _refreshExpiresUtc = currentUtc.Add(timeSpan);
                _refreshTicket = CloneTicket(ticket, replacedPrincipal);
            }
        }

        private AuthenticationTicket CloneTicket(AuthenticationTicket ticket, ClaimsPrincipal replacedPrincipal)
        {
            var principal = replacedPrincipal ?? ticket.Principal;
            var newPrincipal = new ClaimsPrincipal();
            foreach (var identity in principal.Identities)
            {
                newPrincipal.AddIdentity(identity.Clone());
            }

            var newProperties = new AuthenticationProperties();
            foreach (var item in ticket.Properties.Items)
            {
                newProperties.Items[item.Key] = item.Value;
            }

            return new AuthenticationTicket(newPrincipal, newProperties, ticket.AuthenticationScheme);
        }

        private async Task<AuthenticateResult> ReadWebhookTicket()
        {
            var webhookTicket = await _webhookManager.ReadTicket(Request); 

            var ticket = Options.TicketDataFormat.Unprotect(webhookTicket, GetTlsTokenBinding());
            if (ticket == null)
            {
                return AuthenticateResult.Fail("Unprotect ticket failed");
            }

            var currentUtc = Clock.UtcNow;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (expiresUtc != null && expiresUtc.Value < currentUtc)
            {
                return AuthenticateResult.Fail("Ticket expired");
            }

            CheckForRefresh(ticket);

            // Finally we have a valid ticket
            return AuthenticateResult.Success(ticket);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var result = await EnsureWebhookTicket();
            if (!result.Succeeded)
            {
                return result;
            }

            var context = new WebhookValidatePrincipalContext(Context, Scheme, Options, result.Ticket);
            await Events.ValidatePrincipal(context);

            if (context.Principal == null)
            {
                return AuthenticateResult.Fail("No principal.");
            }

            if (context.ShouldRenew)
            {
                RequestRefresh(result.Ticket, context.Principal);
            }

            return AuthenticateResult.Success(new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name));
        }

        protected virtual async Task Finish()
        {
            // Only renew if requested, and neither sign in or sign out was called
            if (!_shouldRefresh || _signInCalled || _signOutCalled)
            {
                return;
            }

            var ticket = _refreshTicket;
            if (ticket != null)
            {
                var properties = ticket.Properties;

                if (_refreshIssuedUtc.HasValue)
                {
                    properties.IssuedUtc = _refreshIssuedUtc;
                }

                if (_refreshExpiresUtc.HasValue)
                {
                    properties.ExpiresUtc = _refreshExpiresUtc;
                }

                var token = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());
                await _webhookManager.RefreshWebhook(ticket, token);
            }
        }

        protected override async Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            properties ??= new AuthenticationProperties();

            _signInCalled = true;

            var signInContext = new WebhookSigningInContext(
                Context,
                Scheme,
                Options,
                user,
                properties);

            DateTimeOffset issuedUtc;
            if (signInContext.Properties.IssuedUtc.HasValue)
            {
                issuedUtc = signInContext.Properties.IssuedUtc.Value;
            }
            else
            {
                issuedUtc = Clock.UtcNow;
                signInContext.Properties.IssuedUtc = issuedUtc;
            }

            if (!signInContext.Properties.ExpiresUtc.HasValue)
            {
                signInContext.Properties.ExpiresUtc = issuedUtc.Add(Options.ExpireTimeSpan);
            }

            await Events.SigningIn(signInContext);

            var ticket = new AuthenticationTicket(signInContext.Principal, signInContext.Properties, signInContext.Scheme.Name);

            var token = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());
            await _webhookManager.RegisterWebhook(ticket, token);

            var signedInContext = new WebhookSignedInContext(
                Context,
                Scheme,
                signInContext.Principal,
                signInContext.Properties,
                Options);

            await Events.SignedIn(signedInContext);

            Logger.AuthenticationSchemeSignedIn(Scheme.Name);
        }

        protected override async Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            properties = properties ?? new AuthenticationProperties();

            _signOutCalled = true;

            var context = new WebhookSigningOutContext(
                Context,
                Scheme,
                Options,
                properties);

            await Events.SigningOut(context);
            await _webhookManager.DeleteWebhooks(properties);

            Logger.AuthenticationSchemeSignedOut(Scheme.Name);
        }

        private string GetTlsTokenBinding()
        {
            var binding = Context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }
    }
}
