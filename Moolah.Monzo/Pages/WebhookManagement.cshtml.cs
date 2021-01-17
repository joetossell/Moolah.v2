using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moolah.Monzo.Client;
using Moolah.Monzo.Client.Models;

namespace Moolah.Monzo.Pages
{
    public class WebhookManagementModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AccountsClient _accountsClient;
        private readonly WebhookClient _webhookClient;

        public WebhookManagementModel(ILogger<IndexModel> logger, AccountsClient accountsClient, WebhookClient webhookClient)
        {
            _logger = logger;
            _accountsClient = accountsClient;
            _webhookClient = webhookClient;
            Accounts = new Dictionary<string, (Account, Webhook[])>();
        }

        public Dictionary<string, (Account account, Webhook[] webhooks)> Accounts { get; set; }

        public async Task OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var accountsResult = await _accountsClient.ListAccounts();
                var accounts = accountsResult.accounts;
                foreach (var account in accounts)
                {
                    var webhookResult = await _webhookClient.ListWebhooks(account.id);
                    Accounts.Add(account.id, (account, webhookResult.webhooks));
                }
            }
        }

        public async Task OnGetRegister(string accountId)
        {
            var properties = new AuthenticationProperties();
            properties.SetString("accountId", accountId);
            await HttpContext.SignInAsync("Webhook", User, properties);

            await OnGet();
        }

        public async Task OnGetDelete(string accountId, string webhookId)
        {
            var properties = new AuthenticationProperties();
            properties.SetString("accountId", accountId);
            properties.SetString("webhookId", webhookId);
            await HttpContext.SignOutAsync("Webhook", properties);
            await OnGet();
        }
    }
}