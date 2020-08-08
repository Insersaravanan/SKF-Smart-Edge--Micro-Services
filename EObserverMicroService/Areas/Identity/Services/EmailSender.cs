using EMaintanance.Repository;
using EMaintanance.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EMaintanance.Areas.Identity.Services
{
    public class EmailSender : IEmailSender
    {
        private IConfiguration _configuration;
        private readonly Utility util;

        // Get our parameterized configuration
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            util = new Utility(configuration);
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("localhost", 587)
            {
                Credentials = new NetworkCredential("Username", "Password"),
                EnableSsl = true
            };

            return client.SendMailAsync(
                new MailMessage("Username", email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }
    }

    public class EmailConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}