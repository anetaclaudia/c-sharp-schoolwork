using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<SavedGame> SavedGames { get; set; } = default!;
        public DbSet<Player> Players { get; set; } = default!;

        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(
                @"Data Source=/Users/anetaclaudia/RiderProjects/icd0008-2020f/ConsoleApp/database.db");
        }
    }
}