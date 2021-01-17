using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moolah.Authentication.Webhook;
using Moolah.Monzo.Client;
using Moolah.Monzo.Client.Models;
using Moolah.Monzo.Services;

namespace Moolah.Monzo.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = WebhookAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly WhoAmIClient _whoAmI;
        private readonly TransactionClient _transactionClient;
        private readonly PotClient _potClient;
        private readonly EmailService _emailService;

        public WebhookController(WhoAmIClient whoAmI,
            TransactionClient transactionClient,
            PotClient potClient,
            EmailService emailService)
        {
            _whoAmI = whoAmI;
            _transactionClient = transactionClient;
            _potClient = potClient;
            _emailService = emailService;
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> Post(WebhookEvent<Transaction> webhookEvent)
        {
            var whoAmI = await _whoAmI.WhoAmI();
            if (!whoAmI.authenticated)
            {
                _emailService.SendEmail("joetossell29@gmail.com", "Moolah.Monzo: Not authenticated", $"Webhook Authentication failed");
                return Unauthorized();
            }
            var data = webhookEvent.data;
            var transactionResult = await _transactionClient.RetrieveTransaction(data.id);
            var transaction = transactionResult.transaction;
            var validRequest =
                data.amount == transaction.amount
                && data.created == transaction.created
                && data.currency == transaction.currency
                && data.description == transaction.description
                && data.id == transaction.id
                && data.category == transaction.category
                && data.is_load == transaction.is_load;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var dataString = JsonSerializer.Serialize(data, options);
            var transactionString = JsonSerializer.Serialize(transaction, options);

            if (!validRequest)
            {
                _emailService.SendEmail("joetossell29@gmail.com", "Moolah.Monzo: Invalid",
                    @$"Transactions do not match:
{dataString}
{transactionString}");
                return SignOut(WebhookAuthenticationDefaults.AuthenticationScheme);
            }

            var subject = Math.Abs(transaction.amount) > 2000
                ? "Moolah.Monzo: Over 20 quid"
                : "Moolah.Monzo: Under 20 quid";

            //execute rules
            _emailService.SendEmail("joetossell29@gmail.com", subject,
                               @$"Transaction data:
webhookEvent.Type: {webhookEvent.type}
webhook:
{dataString}
api:
{transactionString}");

            if (Math.Abs(transaction.amount) == 123)
            {
                await _potClient.DepositIntoPot("pot_0000A36ecwmrrxj7Nv0nkg", "acc_00009aMpaKEf69daEWah2v", 1, Guid.NewGuid().ToString());
            }

            return Ok();
        }
    }
}
