using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces.Repositories
{
    public interface IPDFRepository
    {
        Task SaveLatestPDFInfoAsync(PDFInfo pdf);
        Task<PDFInfo> GetLatestPDFInfoCreatedAsync();
    }
}