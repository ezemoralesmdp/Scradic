using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces
{
    public interface IWordRepository
    {
        Task SaveWordAsync(Word word);
        Task<List<Word>> GetAllSavedWords();
        Task UpdateWordAsync(Word word);
        bool CheckWordExists(string wordTitle);
        Task<Word> GetWordByTitleAsync(string wordTitle);
        Task<Word> IncrementHints(Word word);
    }
}