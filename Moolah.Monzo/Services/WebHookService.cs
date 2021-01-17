using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moolah.Authentication.Webhook;
using Moolah.Monzo.Client;
using Moolah.Monzo.Client.Models;

namespace Moolah.Monzo.Services
{
    public class WebHookService : IWebhookManager
    {
        public const string QueryStringParameterName = "w";
        private readonly IConfiguration _configuration;
        private readonly WebhookClient _webhookClient;

        public WebHookService(WebhookClient webhookClient, AccountsClient accountsClient, IConfiguration configuration)
        {
            _webhookClient = webhookClient;
            _configuration = configuration;
        }

        public async Task RegisterWebhook(AuthenticationTicket ticket, string token)
        {
            var accountId = ticket.Properties.GetString("accountId");
            if (string.IsNullOrEmpty(accountId)) throw new Exception("RegisterWebhook: accountId is null or empty");

            var baseUri = new Uri(_configuration["Hostname"]);
            var urlBuilder = new UriBuilder(baseUri.Scheme, baseUri.Host, baseUri.Port, "api/Webhook",
                $"?{QueryStringParameterName}={token}");

            await _webhookClient.RegisterWebhook(accountId, urlBuilder.ToString());
        }

        public Task<string> ReadTicket(HttpRequest request)
        {
            if (!request.Query.TryGetValue(QueryStringParameterName, out var values) && values.Count != 1)
                throw new UnauthorizedAccessException();
            return Task.FromResult(values.Single());
        }

        public async Task RefreshWebhook(AuthenticationTicket ticket, string token)
        {
            var accountId = ticket.Properties.GetString("accountId");
            if (string.IsNullOrEmpty(accountId)) throw new Exception("RefreshWebhook: accountId is null or empty");

            var baseUri = new Uri(_configuration["Hostname"]);
            var urlBuilder = new UriBuilder(baseUri.Scheme, baseUri.Host, baseUri.Port, "api/Webhook",
                $"?{QueryStringParameterName}={token}");

            var webhooks = await _webhookClient.ListWebhooks(accountId);
            var appWebHooks = webhooks.webhooks.Where(x => x.url.Contains(baseUri.Host));

            foreach (var appWebHook in appWebHooks)
            {
                await _webhookClient.DeleteWebhook(appWebHook.id);
                await _webhookClient.RegisterWebhook(accountId, urlBuilder.ToString());
            }
        }

        public async Task DeleteWebhooks(AuthenticationProperties properties)
        {
            var accountId = properties.GetString("accountId");
            if (string.IsNullOrEmpty(accountId)) throw new Exception("DeleteWebhooks: accountId is null or empty");

            var webhooksId = properties.GetString("webhookId");
            var webhooks = !string.IsNullOrEmpty(webhooksId) 
                ? new[] {webhooksId} 
                : (await _webhookClient.ListWebhooks(accountId)).webhooks.Select(w => w.id);

            foreach (var webhook in webhooks) await _webhookClient.DeleteWebhook(webhook);
        }
    }
}