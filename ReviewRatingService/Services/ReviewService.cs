using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReviewRatingService.Models;
using ReviewRatingService.Interfaces;
using ReviewRatingService.Data;

namespace ReviewRatingService.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddReview(ReviewDto reviewDto)
        {
            try
            {
                var review = new Review
                {
                    PropertyId = reviewDto.PropertyId,
                    UserId = reviewDto.UserId,
                    Comment = reviewDto.Comment,
                    Rating = reviewDto.Rating,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteReview(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Review?> UpdateReview(int reviewId, ReviewDto reviewDto)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return null;

            review.Comment = reviewDto.Comment;
            review.Rating = reviewDto.Rating;
            review.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review?> GetReviewDetails(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.Property)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<List<Review>> GetAllReviews(int propertyId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task<double> GetAverageRating(int propertyId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.PropertyId == propertyId)
                .Select(r => r.Rating)
                .ToListAsync();

            if (!ratings.Any())
                return 0;

            return ratings.Average();
        }

        public async Task<List<Review>> GetUserReviews(int userId)
        {
            return await _context.Reviews
                .Include(r => r.Property)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
    }
}