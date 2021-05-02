using System.Collections.Generic;

namespace Domain
{
    public class Location
    {
        public Location()
        {
        }

        public int? LocationId { get; set; }
        public string? LocationName { get; set; }

        public ICollection<Ingredient>? Ingredients { get; set; }
    }
}