using Microsoft.AspNetCore.Mvc;
using api.Dtos;
using api.Interfaces.Services;
using MongoDB.Bson;
using System.Threading.Tasks;
using api.Services.Customer;
using Microsoft.AspNetCore.Authorization;
using api.Utils;
using System.Security.Claims;
using api.Interfaces.Repositories;
using api.Models;

namespace api.Controllers
{
    [Route("v1/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly INotificationRepository _notificationRepository;

        public ReviewsController(IReviewService reviewService, INotificationRepository notificationRepository)
        {
            _reviewService = reviewService;
            _notificationRepository = notificationRepository;
        }

        [HttpGet("{variantId}")]
        public async Task GetReviews(string variantId)
        {

            try
            {
                var reviews = await _reviewService.GetReviewsByVariantIdAsync(ObjectId.Parse(variantId));
                await ResponseHandler.SendSuccess(Response, reviews, 200, "Reviews fetched successfully");

            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);

            }
        }

        [Authorize]
        [HttpPost]
        public async Task CreateReview([FromBody] CreateReviewDto reviewDto)
        {
            var userId = User.FindFirst("userId")?.Value;

            try
            {
                var savedReview = await _reviewService.CreateReviewAsync(reviewDto, ObjectId.Parse(userId));
                await _notificationRepository.Create(new Notification
                {
                    title = "📝 Review mới",
                    message = $"Người dùng {userId} vừa đánh giá variant {savedReview.variant} với {savedReview.rating}⭐",
                    type = "review",
                    targetRole = "admin",
                    isRead = false,
                    createdAt = DateTime.UtcNow
                });
                await ResponseHandler.SendSuccess(Response, savedReview, 201, "Review created successfully");

            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);

            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task UpdateReview(string id, [FromBody] UpdateReviewDto reviewDto)
        {

            var userId = User.FindFirst("userId")?.Value;
            try
            {
                var updatedReview = await _reviewService.UpdateReviewAsync(ObjectId.Parse(id), ObjectId.Parse(userId), reviewDto);
                await _notificationRepository.Create(new Notification
                {
                    title = "✏️ Review được cập nhật",
                    message = $"Người dùng {userId} đã cập nhật đánh giá variant {updatedReview.variant} thành {updatedReview.rating}⭐",
                    type = "review",
                    targetRole = "admin",
                    isRead = false,
                    createdAt = DateTime.UtcNow
                });
                await ResponseHandler.SendSuccess(Response, updatedReview, 200, "Review updated successfully");

            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);

            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task DeleteReview(string id)
        {

            var userId = User.FindFirst("userId")?.Value;

            try
            {
                var deletedReview = await _reviewService.DeleteReviewAsync(ObjectId.Parse(id), ObjectId.Parse(userId));
                await _notificationRepository.Create(new Notification
                {
                    title = "❌ Review bị xoá",
                    message = $"Người dùng {userId} đã xoá đánh giá variant {deletedReview.variant} ({deletedReview.rating}⭐)",
                    type = "review",
                    targetRole = "admin",
                    isRead = false,
                    createdAt = DateTime.UtcNow
                });
                await ResponseHandler.SendSuccess(Response, deletedReview, 200, "Review deleted successfully");

            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);

            }
        }
    }
}