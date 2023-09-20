namespace Scradic.Core.Entities
{
    public class EmailRequest
    {
        public string? ToEmail { get; set; } 
        public string? Subject { get; set; } 
        public string? Body { get; set; } 
        public string? PDFPath { get; set; } 
        public string? PDFFileName { get; set; } 
        public string? LogoBase64 { get; set; }
    }
}