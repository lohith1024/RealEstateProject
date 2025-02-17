using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BookingService.Models;
using BookingService.Models.DTOs;
using BookingService.Data;
using BookingService.Interfaces;

namespace BookingService.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookingDto> CreateBooking(BookingDto bookingDto)
        {
            var booking = new Booking
            {
                UserId = bookingDto.UserId,
                PropertyId = bookingDto.PropertyId,
                CheckInDate = bookingDto.CheckInDate,
                CheckOutDate = bookingDto.CheckOutDate,
                TotalPrice = bookingDto.TotalPrice,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return MapToDto(booking);
        }

        public async Task<BookingDto?> UpdateBooking(int id, BookingDto bookingDto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return null;

            booking.CheckInDate = bookingDto.CheckInDate;
            booking.CheckOutDate = bookingDto.CheckOutDate;
            booking.Status = bookingDto.Status;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(booking);
        }

        public async Task<BookingDto?> GetBookingById(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Property)
                .FirstOrDefaultAsync(b => b.Id == id);

            return booking == null ? null : MapToDto(booking);
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookings()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Property)
                .Select(b => MapToDto(b))
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingDto>> GetUserBookings(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Property)
                .Where(b => b.UserId == userId)
                .Select(b => MapToDto(b))
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingDto>> GetPropertyBookings(int propertyId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Where(b => b.PropertyId == propertyId)
                .Select(b => MapToDto(b))
                .ToListAsync();
        }

        public async Task<BookingDto?> CancelBooking(int id, string cancellationReason)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return null;

            booking.Status = "Cancelled";
            booking.CancellationReason = cancellationReason;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(booking);
        }

        public async Task<BookingDto?> ConfirmBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return null;

            booking.Status = "Confirmed";
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(booking);
        }

        public async Task<BookingDto?> ProcessPayment(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return null;

            booking.Status = "Paid";
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(booking);
        }

        private static BookingDto MapToDto(Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                PropertyId = booking.PropertyId,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                PaymentId = booking.PaymentId,
                CancellationReason = booking.CancellationReason,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }
    }
}