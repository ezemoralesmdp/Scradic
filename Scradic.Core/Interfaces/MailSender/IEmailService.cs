using Scradic.Services.EmailHelper;

namespace Scradic.Core.Interfaces
{
    public interface IEmailService
    {
        void SendEmailWithAttachmentAsync(MailRequest mailRequest);
    }
}