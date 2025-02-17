using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookingService.Models;
using BookingService.Models.DTOs;
using BookingService.Interfaces;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookings();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingById(id);
            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetUserBookings(int userId)
        {
            var bookings = await _bookingService.GetUserBookings(userId);
            return Ok(bookings);
        }

        [HttpGet("property/{propertyId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetPropertyBookings(int propertyId)
        {
            var bookings = await _bookingService.GetPropertyBookings(propertyId);
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(BookingDto bookingDto)
        {
            var booking = await _bookingService.CreateBooking(bookingDto);
            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookingDto>> UpdateBooking(int id, BookingDto bookingDto)
        {
            var booking = await _bookingService.UpdateBooking(id, bookingDto);
            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<BookingDto>> CancelBooking(int id, [FromBody] string reason)
        {
            var booking = await _bookingService.CancelBooking(id, reason);
            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        [HttpPost("{id}/confirm")]
        public async Task<ActionResult<BookingDto>> ConfirmBooking(int id)
        {
            var booking = await _bookingService.ConfirmBooking(id);
            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        [HttpPost("{id}/process-payment")]
        public async Task<ActionResult<BookingDto>> ProcessPayment(int id)
        {
            var result = await _bookingService.ProcessPayment(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}