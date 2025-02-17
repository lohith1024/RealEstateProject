using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SearchService.Models;
using SearchService.Interfaces;
using SearchService.Data;

namespace SearchService.Services
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Property>> SearchProperties(SearchCriteria criteria)
        {
            var query = _context.Properties.AsQueryable();

            if (!string.IsNullOrEmpty(criteria.Location))
            {
                query = query.Where(p => p.Location.Contains(criteria.Location));
            }

            if (criteria.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= criteria.MinPrice.Value);
            }

            if (criteria.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= criteria.MaxPrice.Value);
            }

            if (criteria.IsAvailable.HasValue)
            {
                query = query.Where(p => p.IsAvailable == criteria.IsAvailable.Value);
            }

            if (criteria.PropertyType.HasValue)
            {
                query = query.Where(p => p.Type == criteria.PropertyType.Value);
            }

            if (criteria.CheckInDate.HasValue && criteria.CheckOutDate.HasValue)
            {
                query = query.Where(p => !p.Bookings.Any(b =>
                    (b.CheckInDate <= criteria.CheckOutDate && b.CheckOutDate >= criteria.CheckInDate) &&
                    b.Status != BookingStatus.Cancelled));
            }

            return await query.ToListAsync();
        }
    }
}