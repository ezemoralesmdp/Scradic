namespace Scradic.Core.Interfaces.Services
{
    public interface IPDFService
    {
        Task CreatePDF();
        Task SeePDFList();
    }
}