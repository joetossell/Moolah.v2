using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moolah.Monzo.Services;

namespace Moolah.Monzo.Pages
{
    public class EmailModel : PageModel
    {
        private UserManager<IdentityUser> _userManager;
        private EmailService _emailService;

        public EmailModel(
            UserManager<IdentityUser> userManager,
            EmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public string Email { get; set; }

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                Email = user.Email;
            }
        }

        public async Task OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            _emailService.SendEmail(user.Email, "Test email", "It works!");
            Email = user.Email;
        }
    }
}
