using Services.Common;
using System.Net.Mail;

namespace Services.SendEmail
{
    public interface IEmailService : IService
    {
        Task SendEmailAsync(string email, string subject, string body);
        Task SendEmailWithAtachmentAsync(string email, string subject, string body,Attachment atachment);
    }
}
