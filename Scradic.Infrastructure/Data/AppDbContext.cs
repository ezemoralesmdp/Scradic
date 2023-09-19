using Microsoft.EntityFrameworkCore;
using Scradic.Core.Entities;

namespace Scradic.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Definition> Definition { get; set; }
        public DbSet<Word> Word { get; set; }
        public DbSet<Example> Example { get; set; }
        public string DbPath { get; }

        public AppDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "Scradic.db");
            Database.EnsureCreated();
        }

        // The following configures EF to create a Sqlite database file in the special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");
    }
}