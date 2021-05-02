using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext:DbContext
    {
        public DbSet<IngredientInRecipe>? IngredientInRecipes { get; set; }
        public DbSet<Location>? Locations { get; set; }
        public DbSet<Recipe>? Recipes { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(
                @"Data Source=/Users/anetaclaudia/RiderProjects/icd0008-2020f/Exam/WebApplication/database.db");
        }
    }
}