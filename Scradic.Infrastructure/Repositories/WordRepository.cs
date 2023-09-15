using Microsoft.EntityFrameworkCore;
using Scradic.Core.Entities;
using Scradic.Core.Interfaces;
using Scradic.Infrastructure.Data;

namespace Scradic.Infrastructure.Repositories
{
    public class WordRepository : IWordRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Word> _entity;

        public WordRepository(AppDbContext context)
        {
            _context = context;
            _entity = context.Set<Word>();
        }

        public async Task<Word> IncrementHints(Word word)
        {
            word.Hits++;
            _entity.Update(word);
            await _context.SaveChangesAsync();
            return word;
        }

        public bool CheckWordExists(string wordTitle)
        {
            var query = _entity.AsQueryable().AsNoTracking();
            return query.Any(w => w.Title.ToLower() == wordTitle);
        }

        public async Task<Word> GetWordByTitleAsync(string wordTitle)
        {
            var query = _entity.Include(w => w.Definitions).Include(w => w.Examples).AsQueryable().AsNoTracking();
            query = query.Where(w => w.Title.ToLower() == wordTitle);
            return await query.FirstOrDefaultAsync();
        }

        public async Task SaveWordAsync(Word word)
        {
            _entity.Add(word);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWordAsync(Word word)
        {
            _entity.Update(word);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Word>> GetAllSavedWords()
        {
            return await _entity.Include(w => w.Definitions).Include(w => w.Examples).AsQueryable().AsNoTracking().ToListAsync();
        }

        public async Task<List<Word>> GetTop(int amount)
        {
            return await _entity.AsQueryable().AsNoTracking().Take(amount).ToListAsync();
        }
    }
}