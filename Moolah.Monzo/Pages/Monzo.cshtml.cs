using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moolah.Monzo.Client;
using Moolah.Monzo.Client.Models;

namespace Moolah.Monzo.Pages
{
    public class MonzoModel : PageModel
    {
        private readonly ILogger<MonzoModel> _logger;
        private readonly WhoAmIClient _whoAmIService;
        private readonly AccountsClient _accountsClient;

        public MonzoModel(ILogger<MonzoModel> logger, WhoAmIClient whoAmIService, AccountsClient accountsClient)
        {
            _logger = logger;
            _whoAmIService = whoAmIService;
            _accountsClient = accountsClient;
        }

        public WhoAmI WhoAmI { get; set; }
        public Account[] Accounts { get; set; }

        public async Task OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                WhoAmI = await _whoAmIService.WhoAmI();
                var accountsResult = await _accountsClient.ListAccounts();
                Accounts = accountsResult.accounts;
            }
        }
    }
}