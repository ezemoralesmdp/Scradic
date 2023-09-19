using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scradic.Services.EmailHelper
{
    public class MailRequest
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string PDFPath { get; set; } = string.Empty;
        public string PDFFileName { get; set; } = string.Empty;
        public string LogoBase64 { get; set; } = string.Empty;
    }
}
