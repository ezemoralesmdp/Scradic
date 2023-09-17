using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces
{
    public interface IWordRepository
    {
        Task<Word> IncrementHints(Word word);
        bool CheckWordExists(string wordTitle);
        Task<Word> GetWordByTitleAsync(string wordTitle);
        Task SaveWordAsync(Word word);
        Task UpdateWordAsync(Word word);
        Task<List<Word>> GetAllSavedWordsOrderByDescendingAsync();
        Task<List<Word>> GetTop(int amount);
        Task<List<Word>> GetAllToPdfAsync();
    }
}