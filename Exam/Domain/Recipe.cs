using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Recipe
    {
        public Recipe()
        {
        }

        public int? RecipeId { get; set; }
        public string? RecipeName { get; set; }
        public RecipeCategory? RecipeCategory { get; set; }
        public int? PreparationTime { get; set; }

        public string? Preparation { get; set; }

        public ICollection<IngredientInRecipe>? RecipeIngredients { get; set; }

        public Recipe(string? recipeName, RecipeCategory? recipeCategory, int? preparationTime)
        {
            RecipeName = recipeName;
            RecipeCategory = recipeCategory;
            PreparationTime = preparationTime;
        }
    }
    
}