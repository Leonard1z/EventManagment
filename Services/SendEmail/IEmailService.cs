using Services.Common;

namespace Services.SendEmail
{
    public interface IEmailService : IService
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}
