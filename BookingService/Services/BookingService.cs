using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BookingService.Models;
using BookingService.Models.DTOs;
using BookingService.Data;
using BookingService.Interfaces;
using BookingService.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingService.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly IPaymentService _paymentService;
        private readonly IPropertyService _propertyService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository repository,
            IPaymentService paymentService,
            IPropertyService propertyService,
            INotificationService notificationService,
            ILogger<BookingService> logger)
        {
            _repository = repository;
            _paymentService = paymentService;
            _propertyService = propertyService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<BookingDto> CreateBooking(BookingDto bookingDto)
        {
            try
            {
                // Verify property availability
                var property = await _propertyService.GetPropertyAsync(bookingDto.PropertyId);
                if (property == null)
                {
                    throw new InvalidOperationException("Property not found");
                }

                var booking = new Booking
                {
                    UserId = bookingDto.UserId,
                    PropertyId = bookingDto.PropertyId,
                    CheckInDate = bookingDto.CheckInDate,
                    CheckOutDate = bookingDto.CheckOutDate,
                    TotalPrice = bookingDto.TotalPrice,
                    Status = "Pending"
                };

                // Create booking
                var createdBooking = await _repository.CreateBookingAsync(booking);

                return MapToDto(createdBooking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                throw;
            }
        }

        public async Task<BookingDto?> UpdateBooking(int id, BookingDto bookingDto)
        {
            var booking = await _repository.GetBookingAsync(id);
            if (booking == null) return null;

            booking.CheckInDate = bookingDto.CheckInDate;
            booking.CheckOutDate = bookingDto.CheckOutDate;
            booking.TotalPrice = bookingDto.TotalPrice;
            booking.UpdatedAt = DateTime.UtcNow;

            var success = await _repository.UpdateBookingAsync(booking);
            return success ? MapToDto(booking) : null;
        }

        public async Task<BookingDto?> GetBookingById(int id)
        {
            var booking = await _repository.GetBookingAsync(id);
            return booking != null ? MapToDto(booking) : null;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookings()
        {
            var bookings = await _repository.GetBookingsByUserAsync(0); // TODO: Implement GetAllBookings in repository
            return bookings.Select(MapToDto);
        }

        public async Task<IEnumerable<BookingDto>> GetUserBookings(int userId)
        {
            var bookings = await _repository.GetBookingsByUserAsync(userId);
            return bookings.Select(MapToDto);
        }

        public async Task<IEnumerable<BookingDto>> GetPropertyBookings(int propertyId)
        {
            var bookings = await _repository.GetBookingsByPropertyAsync(propertyId);
            return bookings.Select(MapToDto);
        }

        public async Task<BookingDto?> CancelBooking(int id, string cancellationReason)
        {
            var booking = await _repository.GetBookingAsync(id);
            if (booking == null) return null;

            booking.Status = "Cancelled";
            booking.UpdatedAt = DateTime.UtcNow;

            var success = await _repository.UpdateBookingAsync(booking);
            if (success)
            {
                await _notificationService.SendBookingCancellationAsync(id);
                return MapToDto(booking);
            }
            return null;
        }

        public async Task<BookingDto?> ConfirmBooking(int id)
        {
            var booking = await _repository.GetBookingAsync(id);
            if (booking == null) return null;

            booking.Status = "Confirmed";
            booking.UpdatedAt = DateTime.UtcNow;

            var success = await _repository.UpdateBookingAsync(booking);
            if (success)
            {
                await _notificationService.SendBookingConfirmationAsync(id);
                return MapToDto(booking);
            }
            return null;
        }

        public async Task<BookingDto?> ProcessPayment(int id)
        {
            var booking = await _repository.GetBookingAsync(id);
            if (booking == null) return null;

            var paymentResult = await _paymentService.ProcessPaymentAsync(new PaymentRequest
            {
                Amount = booking.TotalPrice,
                UserId = booking.UserId,
                BookingId = booking.Id
            });

            if (paymentResult.Success)
            {
                booking.Status = "Paid";
                booking.UpdatedAt = DateTime.UtcNow;

                var success = await _repository.UpdateBookingAsync(booking);
                if (success)
                {
                    await _notificationService.SendPaymentConfirmationAsync(id);
                    return MapToDto(booking);
                }
            }

            return null;
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
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }
    }
}