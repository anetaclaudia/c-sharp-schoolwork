using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages.Locations
{
    public class LocationsModel : PageModel
    {
        public ICollection<Location>? Locations { get; set; }

        public async Task OnGetAsync()
        {
            Locations = await new LocationRepository().GetAllLocations();
        }
    }
}