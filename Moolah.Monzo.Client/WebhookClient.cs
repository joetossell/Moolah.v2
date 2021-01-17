using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Moolah.Monzo.Client.Models;

using Newtonsoft.Json.Serialization;

namespace Moolah.Monzo.Client
{
    public class WebhookClient
    {
        public readonly HttpClient _client;

        public WebhookClient(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<WebhookResult> RegisterWebhook(string accountId, string url)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("account_id", accountId),
                new KeyValuePair<string, string>("url", url)
            });
            var result = await _client.PostAsync("webhooks", formContent);
            return await result.Content.ReadFromJsonAsync<WebhookResult>();
        }

        public async Task<WebhooksResult> ListWebhooks(string accountId)
        {
            return await _client.GetFromJsonAsync<WebhooksResult>($"webhooks?account_id={accountId}");
        }

        public Task DeleteWebhook(string webhookId)
        {
            return _client.DeleteAsync($"webhooks/{webhookId}");
        }
    }
}