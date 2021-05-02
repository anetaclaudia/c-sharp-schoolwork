using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages.Recipes
{
    public class AddRecipe : PageModel
    {
        private readonly RecipeRepository _recipeRepository;

        public AddRecipe()
        {
            _recipeRepository = new RecipeRepository();
            Ingredients = new List<IngredientInRecipe>();
        }

        [BindProperty] public string? RecipeName { get; set; }
        [BindProperty] public int? PreparationTime { get; set; }
        [BindProperty] public RecipeCategory? RecipeCategory { get; set; }
        [BindProperty] public List<IngredientInRecipe>? Ingredients { get; set; }
        [BindProperty] public int? ServingAmount { get; set; }
        [BindProperty] public string? Preparation { get; set; }


        public void OnGet()
        {
            
        }
        
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var ingredient in Ingredients!)
            {
                ingredient.AmountPerServing = (int) ingredient.AmountPerServing! / ServingAmount;
            }
            Recipe recipeToAdd = new Recipe(RecipeName, RecipeCategory, PreparationTime);
            recipeToAdd.RecipeIngredients = Ingredients;
            recipeToAdd.PreparationTime = PreparationTime;
            recipeToAdd.Preparation = Preparation;
            _recipeRepository.AddRecipe(recipeToAdd);
            await _recipeRepository.SaveChangesAsync();
            
            return RedirectToPage("./Recipes");
        }
    }
}