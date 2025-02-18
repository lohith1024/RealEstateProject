using System.Threading.Tasks;
using BookingService.Models;

namespace BookingService.Interfaces
{
    public interface IPropertyService
    {
        Task<Property?> GetPropertyAsync(int propertyId);
    }
}