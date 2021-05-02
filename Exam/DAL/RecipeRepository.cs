using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DAL
{
    public class RecipeRepository
    {
        private readonly AppDbContext _context;

        public RecipeRepository()
        {
            _context = new AppDbContext();
        }

        public async Task<ICollection<Recipe>> GetAllRecipes()
        {
            var recipes = await _context.Recipes!
                .Include(x => x.RecipeIngredients)
                .ToListAsync();
            return recipes;
        }

        public async Task<Recipe> GetRecipe(int? id)
        {
            var recipe = await _context.Recipes!
                .Include(x => x.RecipeIngredients)
                .FirstOrDefaultAsync(recipe1 => recipe1.RecipeId == id);
            return recipe;
        }

        public void AddRecipe(Recipe recipe)
        {
            _context.Update(recipe);
        }
        
        public void DeleteRecipe(Recipe recipe)
        {
            _context.Remove(recipe);
        }
        
        public void UpdateRecipe(Recipe recipe)
        {
            _context.Update(recipe);
        }
        
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}