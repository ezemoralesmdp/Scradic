using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces.Services
{
    public interface IPDFService
    {
        Task CreatePDF();
        Task SeePDFList();
        Task<PDFInfo> GetLatestPDFInfoCreatedAsync();
    }
}