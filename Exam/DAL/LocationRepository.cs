using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class LocationRepository
    {
        private readonly AppDbContext _context;

        public LocationRepository()
        {
            _context = new AppDbContext();
        }
        
        public async Task<ICollection<Location>> GetAllLocations()
        {
            var locations = await _context.Locations!.ToListAsync();
            return locations;
        }
        
        public async Task<Location> GetLocation(int? id)
        {
            var location = await _context.Locations!
                .Include(x => x.Ingredients)
                .FirstOrDefaultAsync(location1 => location1.LocationId == id);
            return location;
        }
        
        public void AddLocation(Location? location)
        {
            _context.Locations!.Add(location!);
        }
        
        public void UpdateLocation(Location? location)
        {
            _context.Locations!.Update(location!);
        }
        
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}