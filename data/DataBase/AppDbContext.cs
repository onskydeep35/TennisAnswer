using Microsoft.EntityFrameworkCore;
using Data.Models.Tennis;
using Data.Models;

namespace DataBase
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; } 
        public DbSet<Match> Matches { get; set; }
    }
}

