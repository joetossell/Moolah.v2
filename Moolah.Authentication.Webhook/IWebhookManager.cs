using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Moolah.Authentication.Webhook
{
    public interface IWebhookManager
    {
        Task RegisterWebhook(AuthenticationTicket ticket, string token);
        Task RefreshWebhook(AuthenticationTicket ticket, string token);
        Task DeleteWebhooks(AuthenticationProperties properties);
        Task<string> ReadTicket(HttpRequest request);
    }
}
