using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.Pages.Recipes
{
    public class RecipeDetails : PageModel
    {
        private readonly RecipeRepository? _repository;
        public Recipe Recipe { get; set; } = default!;
        
        [BindProperty] public int? ServingPeopleAmount { get; set; }

        public RecipeDetails()
        {
            _repository = new RecipeRepository();
        }

        public async Task<IActionResult> OnGetAsync(int? recipeId)
        {
            if (recipeId == null)
            {
                return NotFound();
            }

            Recipe = await _repository!.GetRecipe(recipeId);
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int? recipeId)
        {
            if (recipeId == null)
            {
                return NotFound();
            }

            Recipe = await _repository!.GetRecipe(recipeId);

            return Page();
        }
    }
}