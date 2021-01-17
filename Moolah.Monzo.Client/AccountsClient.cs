using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Moolah.Monzo.Client.Models;

namespace Moolah.Monzo.Client
{
    public class AccountsClient
    {
        private readonly HttpClient _client;

        public AccountsClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<AccountsResult> ListAccounts()
        {
            return await _client.GetFromJsonAsync<AccountsResult>("accounts");
        }
    }
}
