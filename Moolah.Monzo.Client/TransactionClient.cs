using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Moolah.Monzo.Client.Models;
using Moolah.Monzo.Client.Serialization;

namespace Moolah.Monzo.Client
{
    public class TransactionClient
    {
        private readonly HttpClient _client;

        public TransactionClient(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<TransactionResult> RetrieveTransaction(string id)
        {
            return await _client.GetFromJsonAsync<TransactionResult>($"transactions/{id}?expand[]=merchant");
        }

        public async Task<TransactionResult> ListTransactions(string accountId, bool isFirstFiveMinutes = false)
        {
            var url = $"transactions/?expand[]=merchant&account_id={accountId}{(isFirstFiveMinutes ? string.Empty : $"&since={DateTimeOffset.Now.AddDays(-90)}")}";
            return await _client.GetFromJsonAsync<TransactionResult>(url, new JsonSerializerOptions
            {
                Converters = { new EmptyDateTimeOffsetOffsetToNullConverter()}
            });
        }
    }
}
