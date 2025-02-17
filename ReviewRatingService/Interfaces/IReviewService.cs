using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewRatingService.Models;

namespace ReviewRatingService.Interfaces
{
    public interface IReviewService
    {
        Task<bool> AddReview(ReviewDto reviewDto);
        Task<bool> DeleteReview(int reviewId);
        Task<Review?> UpdateReview(int reviewId, ReviewDto reviewDto);
        Task<Review?> GetReviewDetails(int reviewId);
        Task<List<Review>> GetAllReviews(int propertyId);
        Task<double> GetAverageRating(int propertyId);
        Task<List<Review>> GetUserReviews(int userId);
    }
}