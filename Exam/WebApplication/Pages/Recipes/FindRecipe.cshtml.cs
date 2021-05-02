using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.Pages.Recipes
{
    public class FindRecipe : PageModel
    {

        private readonly RecipeRepository _recipeRepository;
        private readonly LocationRepository _locationRepository;

        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchIngredientPositive { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? SearchIngredientNegative { get; set; }
        [BindProperty]public RecipeCategory? RecipeCategory { get; set; }
        [BindProperty]public int? LocationId { get; set; }
        [BindProperty]public int? SearchTime { get; set; }

        public ICollection<Recipe>? Recipes { get; set; }
        public ICollection<Location>? Locations { get; set; }
        
        public FindRecipe()
        {
            _recipeRepository = new RecipeRepository();
            _locationRepository = new LocationRepository();
        }
        

        public async Task OnGetAsync()
        {
            Recipes = await _recipeRepository.GetAllRecipes();
            Locations = await _locationRepository.GetAllLocations();
        }
        
        public async Task OnPostAsync()
        {
            Recipes = await _recipeRepository.GetAllRecipes();
            Locations = await _locationRepository.GetAllLocations();
            if (SearchName != null)
            {
                Recipes = Recipes.Where(recipe => recipe.RecipeName!.Contains(SearchName)).ToList();
            }
            if (SearchIngredientPositive != null)
            {
                Recipes = Recipes.Where(recipe => 
                    recipe.RecipeIngredients!
                    .Any(ingredient => 
                        ingredient.Name!.Contains(SearchIngredientPositive))).ToList();
            }
            
            if (SearchIngredientNegative != null)
            {
                Recipes = Recipes.Where(recipe => 
                    recipe.RecipeIngredients!
                        .All(ingredient => 
                            !ingredient.Name!.Contains(SearchIngredientNegative))).ToList();
            }

            if (SearchTime!= null)
            {
                Recipes = Recipes.Where(recipe => recipe.PreparationTime <= SearchTime).ToList();
            }
            
            if (RecipeCategory != null)
            {
                Recipes = Recipes.Where(recipe => recipe.RecipeCategory.Equals(RecipeCategory)).ToList();
            }

            if (LocationId != null)
            {
                Location location = await _locationRepository.GetLocation(LocationId);
                HashSet<Recipe> result = new HashSet<Recipe>();
                foreach (var ingredient in location.Ingredients!)
                {
                    foreach (var recipe in Recipes)
                    {
                        foreach (var recipeIngredient in recipe.RecipeIngredients!)
                        {
                            if (recipeIngredient.AmountPerServing <= ingredient.Amount && recipeIngredient.Name!.Equals(ingredient.IngredientName))
                            {
                                result.Add(recipe);
                            }
                        }
                    }
                }
                Recipes = result.ToList();
            }
        }
    }
}