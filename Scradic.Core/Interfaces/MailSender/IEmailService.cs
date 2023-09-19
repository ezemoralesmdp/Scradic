using Scradic.Services.EmailHelper;

namespace Scradic.Core.Interfaces.MailSender
{
    public interface IEmailService
    {
        void SendEmailAsync(MailRequest mailRequest);
    }
}
