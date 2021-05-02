using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages.Recipes
{
    public class EditRecipe : PageModel
    {
        
        private readonly RecipeRepository? _repository;

        public EditRecipe()
        {
            _repository = new RecipeRepository();
        }
        [BindProperty] public Recipe? Recipe { get; set; }
        [BindProperty] public List<IngredientInRecipe>? Ingredients { get; set; }

        public async Task<IActionResult> OnGetAsync(int? recipeId)
        {
            if (recipeId == null)
            {
                return NotFound();
            }

            Recipe = await _repository!.GetRecipe(recipeId);
            Ingredients = Recipe.RecipeIngredients!.ToList();
            
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int? recipeId)
        {
            if (Recipe == null)
            {
                return RedirectToPage("./Index");
            }
            var finalIngredients = Ingredients!
                .Where(ingredient => !string.IsNullOrEmpty(ingredient.Name) && ingredient.AmountPerServing != 0 && ingredient.AmountPerServing != null)
                .ToList();
            Recipe dbRecipe = await _repository!.GetRecipe(recipeId);
            dbRecipe.Preparation = Recipe.Preparation;
            dbRecipe.RecipeName = Recipe.RecipeName;
            dbRecipe.RecipeIngredients = finalIngredients;
            dbRecipe.PreparationTime = Recipe.PreparationTime;
            dbRecipe.RecipeCategory = Recipe.RecipeCategory;
            _repository.UpdateRecipe(dbRecipe);
            await _repository?.SaveChangesAsync()!;
            
            return Page();
        }
    }
}