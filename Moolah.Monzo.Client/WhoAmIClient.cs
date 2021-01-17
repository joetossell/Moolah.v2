using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Moolah.Monzo.Client.Models;


namespace Moolah.Monzo.Client
{
    public class WhoAmIClient
    {
        private readonly HttpClient _client;

        public WhoAmIClient(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<WhoAmI> WhoAmI()
        {
            return await _client.GetFromJsonAsync<WhoAmI>("ping/whoami");
        }
    }
}
