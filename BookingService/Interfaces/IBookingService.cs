using BookingService.Models;
using BookingService.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingService.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDto> CreateBooking(BookingDto bookingDto);
        Task<BookingDto?> UpdateBooking(int id, BookingDto bookingDto);
        Task<BookingDto?> GetBookingById(int id);
        Task<IEnumerable<BookingDto>> GetAllBookings();
        Task<IEnumerable<BookingDto>> GetUserBookings(int userId);
        Task<IEnumerable<BookingDto>> GetPropertyBookings(int propertyId);
        Task<BookingDto?> CancelBooking(int id, string cancellationReason);
        Task<BookingDto?> ConfirmBooking(int id);
        Task<BookingDto?> ProcessPayment(int id);
    }
}