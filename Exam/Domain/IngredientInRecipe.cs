using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class IngredientInRecipe
    {
        public int IngredientInRecipeId { get; set; }
        public string? Name { get; set; }
        public Recipe? Recipe { get; set; }
        public int? AmountPerServing { get; set; }
        public Unit? Unit { get; set; }
        
    }
}