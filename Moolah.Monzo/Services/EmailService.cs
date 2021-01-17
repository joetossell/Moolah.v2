using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Moolah.Monzo.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string to, string subject, string text)
        {
            using var message = new MailMessage();
            message.To.Add(new MailAddress(to));
            message.From = new MailAddress(_configuration["Notifications:From"]);
            message.Subject = subject;
            message.Body = text;

            using var client = new SmtpClient(_configuration["Notifications:Host"])
            {
                Port = _configuration.GetValue<int>("Notifications:Port", 25),
                EnableSsl = _configuration.GetValue<bool>("Notifications:EnableSsl", false)
            };
            var username = _configuration.GetValue<string>("Notifications:Username", null);
            var password = _configuration.GetValue<string>("Notifications:Password", null);
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                client.Credentials = new NetworkCredential(username, password);
            }
            client.Send(message);
        }
    }
}