using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces.Repositories
{
    public interface IWordRepository
    {
        Task<Word> IncrementHints(Word word);
        bool CheckWordExists(string wordTitle);
        Task<Word> GetWordByTitleAsync(string wordTitle);
        Task SaveWordAsync(Word word);
        Task UpdateWordAsync(Word word);
        Task UpdateWordPdfAsync(int wordId, bool newPdfValue);
        Task<List<Word>> GetAllSavedWordsInRangeAsync(int start, int? end);
        Task<List<Word>> GetAllSavedWordsAsync();
        Task<List<Word>> GetTop(int amount);
        Task<List<Word>> GetAllToPdfAsync();
        Task<Word> GetWordByIdAsync(int wordId);
        void CleanWordsPDF();
    }
}