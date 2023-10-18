using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces.Services
{
    public interface IWordService
    {
        void ClearConsole();
        void ShowWord(Word word);
        Task<Word> IncrementHints(Word word);
        bool CheckWordExistsAsync(string wordTitle);
        Task<Word> GetWordByTitleAsync(string wordTitle);
        Task SaveWordAsync(Word word);
        Task AddToPdf(Word word);
        Task ShowTop(int amount);
        Task GetAllSavedWordsInRangeAsync(int start, int? end);
        Task GetAllSavedWordsAsync();
        void UpdateLastSearch(Word word);
    }
}