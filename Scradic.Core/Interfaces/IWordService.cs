using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces
{
    public interface IWordService
    {
        void ShowWord(Word word);
        Task SaveWordAsync(Word word);
        Task GetAllSavedWordsToCache();
        Task AddToPdf(Word word);
        bool CheckWordExistsAsync(string wordTitle);
        Task<Word> GetWordByTitleAsync(string wordTitle);
        Task UpdateWordAsync(Word word);
        Task<Word> IncrementHints(Word word);
    }
}