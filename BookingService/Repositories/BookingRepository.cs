using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookingService.Data;
using BookingService.Interfaces;
using BookingService.Models;
using Common.Resilience;

namespace BookingService.Repositories
{
    public class BookingRepository : ResilientRepository<BookingDbContext>, IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Booking?> GetBookingAsync(int id)
        {
            return await ExecuteWithRetryAsync(async () => await _context.Bookings.FindAsync(id));
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
        {
            return await ExecuteWithRetryAsync(async () =>
                await _context.Bookings
                    .Where(b => b.UserId == userId)
                    .ToListAsync());
        }

        public async Task<IEnumerable<Booking>> GetBookingsByPropertyAsync(int propertyId)
        {
            return await ExecuteWithRetryAsync(async () =>
                await _context.Bookings
                    .Where(b => b.PropertyId == propertyId)
                    .ToListAsync());
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            await ExecuteWithRetryAsync(async () =>
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
            });
            return booking;
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                _context.Entry(booking).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }
            });
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return false;
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                return true;
            });
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            return await ExecuteWithRetryAsync(async () =>
                await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Property)
                    .FirstOrDefaultAsync(b => b.Id == id));
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId)
        {
            return await ExecuteWithRetryAsync(async () =>
                await _context.Bookings
                    .Include(b => b.Property)
                    .Where(b => b.UserId == userId)
                    .ToListAsync());
        }

        public async Task<IEnumerable<Booking>> GetPropertyBookingsAsync(int propertyId)
        {
            return await ExecuteWithRetryAsync(async () =>
                await _context.Bookings
                    .Include(b => b.User)
                    .Where(b => b.PropertyId == propertyId)
                    .ToListAsync());
        }
    }
}