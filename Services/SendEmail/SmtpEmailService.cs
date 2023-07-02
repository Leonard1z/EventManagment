using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Services.SendEmail
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _email;
        private readonly string _password;

        public SmtpEmailService(IConfiguration configuration)
        {
            _email = configuration["SmtpEmail"];
            _password = configuration["SmtpPassword"];

            _smtpClient = new SmtpClient("smtp.gmail.com", 587);
            _smtpClient.Credentials = new NetworkCredential(_email, _password);
            _smtpClient.EnableSsl = true;
        }
        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var mailMessage = new MailMessage(_email, email, subject, body);
            mailMessage.IsBodyHtml = true;

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
