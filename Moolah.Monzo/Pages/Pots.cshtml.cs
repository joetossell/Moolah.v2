using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moolah.Monzo.Client;
using Moolah.Monzo.Client.Models;

namespace Moolah.Monzo.Pages
{
    public class PotsModel : PageModel
    {
        private readonly PotClient _potsClient;
        private readonly AccountsClient _accountsClient;

        public IEnumerable<(Account, Pot[])> AccountPots { get; set; }
    
        public PotsModel(PotClient potsClient, AccountsClient accountsClient)
        {
            _potsClient = potsClient;
            _accountsClient = accountsClient;
        }

        public async Task OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var accounts = await _accountsClient.ListAccounts();
                var potTasks = accounts.accounts.Select(async x => (x, (await _potsClient.ListPots(x.id)).pots));
                AccountPots = await Task.WhenAll(potTasks);
            }
        }
    }
}
