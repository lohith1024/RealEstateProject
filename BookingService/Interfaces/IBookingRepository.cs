using System.Collections.Generic;
using System.Threading.Tasks;
using BookingService.Models;

namespace BookingService.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetBookingAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId);
        Task<IEnumerable<Booking>> GetBookingsByPropertyAsync(int propertyId);
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);
    }
}