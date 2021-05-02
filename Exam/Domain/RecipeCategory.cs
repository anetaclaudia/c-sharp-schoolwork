using System.ComponentModel;

namespace Domain
{
    public enum RecipeCategory
    {   
        [Description("Meat and poultry")]
        MeatAndPoultry,
        
        [Description("Pasta and noodles")]
        PastaAndNoodles,
        
        [Description("Seafood")]
        Seafood,
        
        [Description("Desserts")]
        Desserts,
        
        [Description("Salads")]
        Salads,
        
        [Description("Soups")]
        Soups
    }
}