using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages.Locations
{
    public class EditLocation : PageModel
    {
        private readonly LocationRepository? _repository;
        
        [BindProperty] public Location Location { get; set; } = default!;
        [BindProperty] public List<Ingredient>? Ingredients { get; set; }

        public EditLocation()
        {
            _repository = new LocationRepository();
        }
        public async Task<IActionResult> OnGetAsync(int? locationId)
        {
            if (locationId == null)
            {
                return NotFound();
            }

            Location = await _repository!.GetLocation(locationId);
            Ingredients = Location.Ingredients!.ToList();
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int? locationId)
        {
            if (Location == null)
            {
                return RedirectToPage("./Index");
            }
            var finalIngredients = Ingredients!
                .Where(ingredient => !string.IsNullOrEmpty(ingredient.IngredientName) && ingredient.Amount != 0 && ingredient.Amount != null)
                .ToList();

            Location location = await _repository!.GetLocation(locationId);
            location.Ingredients = finalIngredients;
            location.LocationName = Location.LocationName;
            _repository.UpdateLocation(location);
            await _repository?.SaveChangesAsync()!;
            
            return Page();
        }
        
    }
}