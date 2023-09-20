using Microsoft.EntityFrameworkCore;
using Scradic.Core.Entities;
using Scradic.Core.Interfaces.Repositories;
using Scradic.Infrastructure.Data;

namespace Scradic.Infrastructure.Repositories
{
    public class PDFRepository : IPDFRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<PDFInfo> _entity;

        public PDFRepository(AppDbContext context)
        {
            _context = context;
            _entity = _context.Set<PDFInfo>();
        }

        public async Task SaveLatestPDFInfoAsync(PDFInfo pdf)
        {
            _entity.Add(pdf);
            await _context.SaveChangesAsync();
        }

        public async Task<PDFInfo> GetLatestPDFInfoCreatedAsync()
        {
            var query = _entity.AsQueryable().AsNoTracking().OrderByDescending(p => p.Id);
            return await query.FirstOrDefaultAsync(p => p.Id > 0);
        }
    }
}