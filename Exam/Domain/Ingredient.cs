namespace Domain
{
    public class Ingredient
    {
        public int? IngredientId { get; set; }
        public string? IngredientName { get; set; }
        public int? Amount { get; set; }
        public Unit? Unit { get; set; }
        public Location? Location { get; set; }
    }
}