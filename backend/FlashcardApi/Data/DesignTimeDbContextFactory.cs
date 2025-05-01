using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using FlashcardAPI.Models;

namespace FlashcardAPI.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=flashcards.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
