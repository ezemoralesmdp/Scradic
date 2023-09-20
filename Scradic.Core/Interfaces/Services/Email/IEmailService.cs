using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces.Services
{
    public interface IEmailService
    {
        void SendEmailWithAttachmentAsync(EmailRequest mailRequest);
    }
}