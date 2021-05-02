using System.Collections.Generic;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages.Recipes
{
    public class RecipesModel : PageModel
    {
        public ICollection<Recipe>? Recipes { get; set; }

        public async Task OnGetAsync()
        {
            // Recipe recipe = new Recipe("pasta", RecipeCategory.PastaAndNoodles, 10);
            // var recipeRepo = new RecipeRepository();
            // recipeRepo.AddRecipe(recipe);
            // recipeRepo.SaveChangesAsync();
            
            Recipes = await new RecipeRepository().GetAllRecipes();
        }
        
        
    }
}