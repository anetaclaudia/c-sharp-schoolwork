using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages.Locations
{
    public class LocationDetails : PageModel
    {
        private readonly LocationRepository? _repository;
        
        public Location Location { get; set; } = default!;

        public LocationDetails()
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
            
            return Page();
        }
    }
}