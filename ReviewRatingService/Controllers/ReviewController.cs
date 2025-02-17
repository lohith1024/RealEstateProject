using Microsoft.AspNetCore.Mvc;
using ReviewRatingService.Models;
using ReviewRatingService.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewRatingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("property/{propertyId}")]
        public async Task<ActionResult<List<Review>>> GetAllReviews(int propertyId)
        {
            var reviews = await _reviewService.GetAllReviews(propertyId);
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var review = await _reviewService.GetReviewDetails(id);
            if (review == null)
                return NotFound();

            return Ok(review);
        }

        [HttpPost]
        public async Task<ActionResult> CreateReview([FromBody] ReviewDto reviewDto)
        {
            var success = await _reviewService.AddReview(reviewDto);
            if (!success)
                return BadRequest("Failed to create review");

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Review>> UpdateReview(int id, [FromBody] ReviewDto reviewDto)
        {
            var review = await _reviewService.UpdateReview(id, reviewDto);
            if (review == null)
                return NotFound();

            return Ok(review);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReview(int id)
        {
            var result = await _reviewService.DeleteReview(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Review>>> GetUserReviews(int userId)
        {
            var reviews = await _reviewService.GetUserReviews(userId);
            return Ok(reviews);
        }

        [HttpGet("property/{propertyId}/rating")]
        public async Task<ActionResult<double>> GetPropertyAverageRating(int propertyId)
        {
            var rating = await _reviewService.GetAverageRating(propertyId);
            return Ok(rating);
        }
    }
}