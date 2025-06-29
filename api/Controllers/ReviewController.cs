using Microsoft.AspNetCore.Mvc;
using api.Dtos;
using api.Interfaces.Services;
using MongoDB.Bson;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Services.Customer;

namespace api.Controllers
{
    [Route("api/v1/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("{variantId}")]
        public async Task<IActionResult> GetReviews(string variantId)
        {
            if (!ObjectId.TryParse(variantId, out var parsedVariantId))
            {
                return BadRequest("Invalid variant ID format");
            }

            try
            {
                var reviews = await _reviewService.GetReviewsByVariantIdAsync(parsedVariantId);
                return Ok(new { Message = "Reviews fetched successfully", Reviews = reviews });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto reviewDto)
        {
            
            var userSub = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userSub))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            if (!ObjectId.TryParse(userSub, out var userId))
            {
                return BadRequest(new { Message = "Invalid user ID format" });
            }

            try
            {
                var savedReview = await _reviewService.CreateReviewAsync(reviewDto, userId);
                return StatusCode(201, new { Message = "Review created successfully", Data = savedReview });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(string id, [FromBody] UpdateReviewDto reviewDto)
        {
            if (!ObjectId.TryParse(id, out var parsedId))
            {
                return BadRequest("Invalid review ID format");
            }

            // Kiểm tra user đã authenticate chưa
            var userSub = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userSub))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            if (!ObjectId.TryParse(userSub, out var userId))
            {
                return BadRequest(new { Message = "Invalid user ID format" });
            }

            try
            {
                var updatedReview = await _reviewService.UpdateReviewAsync(parsedId, userId, reviewDto);
                return Ok(new { Message = "Review updated successfully", Data = updatedReview });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(string id)
        {
            if (!ObjectId.TryParse(id, out var parsedId))
            {
                return BadRequest("Invalid review ID format");
            }

            // Kiểm tra user đã authenticate chưa
            var userSub = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userSub))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            if (!ObjectId.TryParse(userSub, out var userId))
            {
                return BadRequest(new { Message = "Invalid user ID format" });
            }

            try
            {
                var deletedReview = await _reviewService.DeleteReviewAsync(parsedId, userId);
                return Ok(new { Message = "Review deleted successfully", Data = deletedReview });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}