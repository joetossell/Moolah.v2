using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Moolah.Monzo.Client.Models;

namespace Moolah.Monzo.Client
{
    public class PotClient
    {
        private readonly HttpClient _client;

        public PotClient(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<PotsResult> ListPots(string currentAccountId)
        {
            return await _client.GetFromJsonAsync<PotsResult>($"pots?current_account_id={currentAccountId}");
        }

        public async Task<Pot> RetrievePot(string potId)
        {
            return await _client.GetFromJsonAsync<Pot>($"pots/{potId}");
        }

        public async Task<Pot> DepositIntoPot(string potId, string sourceAccountId, int amount, string dedupeId)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("source_account_id", sourceAccountId),
                new KeyValuePair<string, string>("amount", amount.ToString()),
                new KeyValuePair<string, string>("dedupe_id", dedupeId)
            });
            var result = await _client.PutAsync($"pots/{potId}/deposit", formContent);
            return await result.Content.ReadFromJsonAsync<Pot>();
        }

        public async Task<Pot> WithdrawFromPot(string potId, string destinationAccountId, int amount, string dedupeId)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("destination_account_id", destinationAccountId),
                new KeyValuePair<string, string>("amount", amount.ToString()),
                new KeyValuePair<string, string>("dedupe_id", dedupeId)
            });
            var result = await _client.PutAsync($"pots/{potId}/withdraw", formContent);
            return await result.Content.ReadFromJsonAsync<Pot>();
        }
    }
}
