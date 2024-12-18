﻿using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Services.SendEmail
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _email;
        private readonly string _password;

        public SmtpEmailService()
        {
            _email = Environment.GetEnvironmentVariable("SMTP_EMAIL");
            _password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

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

        public async Task SendEmailWithAtachmentAsync(string email, string subject, string body, Attachment atachment)
        {
           var mailMessage = new MailMessage(_email,email,subject, body);
            mailMessage.IsBodyHtml = true;
            mailMessage.Attachments.Add(atachment);

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
