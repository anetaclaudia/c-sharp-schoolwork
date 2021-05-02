using System.Collections.Generic;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages.Locations
{
    public class AddLocation : PageModel
    {
        private readonly LocationRepository _locationRepository;
        
        public AddLocation()
        {
            Ingredients = new List<Ingredient>();
            _locationRepository = new LocationRepository();
        }

        [BindProperty] public List<Ingredient>? Ingredients { get; set; }
        
        [BindProperty] public string? LocationName { get; set; }
        
        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Location location = new Location();
            location.LocationName = LocationName;
            location.Ingredients = Ingredients;
            _locationRepository.AddLocation(location);
            await _locationRepository.SaveChangesAsync();
            return RedirectToPage("./Locations");
        }
    }
}