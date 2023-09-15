using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces
{
    public interface IWordService
    {
        void ShowWord(Word word);
        Task<Word> IncrementHints(Word word);
        bool CheckWordExistsAsync(string wordTitle);
        Task<Word> GetWordByTitleAsync(string wordTitle);
        Task SaveWordAsync(Word word);
        Task AddToPdf(Word word);
        Task ShowTop(int amount);
    }
}