using System.Net;
using System.Net.Mail;

namespace Services.SendEmail
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public SmtpEmailService()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com", 587);
            _smtpClient.Credentials = new NetworkCredential("leonardzaberxha@gmail.com", "pfnjoqrqyrlbgdxj");
            _smtpClient.EnableSsl = true;
        }
        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var mailMessage = new MailMessage("leonardzaberxha@gmail.com", email, subject, body);
            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
