using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Scradic.Core.Interfaces;
using Scradic.Services.EmailHelper;

namespace Scradic.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmailWithAttachmentAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("Email:Username").Value));
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = mailRequest.Body,
            };

            var pdf = new MimePart()
            {
                Content = new MimeContent(File.OpenRead(mailRequest.PDFPath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = mailRequest.PDFFileName
            };

            // Cadena Base64 a bytes
            byte[] base64Bytes = Convert.FromBase64String(mailRequest.LogoBase64);

            var imagePart = new MimePart()
            {
                Content = new MimeContent(new MemoryStream(base64Bytes), ContentEncoding.Default),
                ContentId = "<logo>", // Identificador en el HTML
                ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                ContentTransferEncoding = ContentEncoding.Base64
            };
            imagePart.Headers.Add("Content-Location", "cid:logo"); // Encabezado Content-Location

            var multipart = new Multipart("mixed");
            multipart.Add(email.Body);
            multipart.Add(pdf);
            multipart.Add(imagePart);

            email.Body = multipart;

            using var smtp = new SmtpClient();
            smtp.Connect(
                _config.GetSection("Email:Host").Value,
                Convert.ToInt32(_config.GetSection("Email:Port").Value),
                SecureSocketOptions.StartTls
            );

            smtp.Authenticate(_config.GetSection("Email:Username").Value, _config.GetSection("Email:Password").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}