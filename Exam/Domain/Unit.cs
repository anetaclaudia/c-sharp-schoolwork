using System.ComponentModel;

namespace Domain
{
    public enum Unit
    {
        [Description("Teaspoon(s)")]
        Teaspoon,
        [Description("Tablespoon(s)")]
        Tablespoon,
        [Description("Gram(s)")]
        Gram,
        [Description("Millilitre(s)")]
        Millilitre
    }
}