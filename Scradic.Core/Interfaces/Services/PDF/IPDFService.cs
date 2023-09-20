using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces.Services
{
    public interface IPDFService
    {
        Task CreatePDF();
        void SeePDFList();
        Task<PDFInfo> GetLatestPDFInfoCreatedAsync();
        Task AddToPdf(int wordId);
    }
}