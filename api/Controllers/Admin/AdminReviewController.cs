using Microsoft.AspNetCore.Mvc;
using api.Dtos;
using api.Interfaces.Services;
using MongoDB.Bson;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using api.Utils;

namespace api.Controllers.Admin
{
    [Route("api/admin/reviews")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminReviewController : ControllerBase
    {
        private readonly IAdminReviewService _service;
        public AdminReviewController(IAdminReviewService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task GetAll()
        {
            try
            {
                var reviews = await _service.GetAllReviewsAsync();
                await ResponseHandler.SendSuccess(Response, reviews, 200, "All reviews fetched successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpGet("{id}")]
        public async Task GetById(string id)
        {
            try
            {
                var review = await _service.GetReviewByIdAsync(ObjectId.Parse(id));
                await ResponseHandler.SendSuccess(Response, review, 200, "Review fetched successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpGet("variant/{variant}")]
        public async Task GetByVariant(string variant)
        {
            try
            {
                var reviews = await _service.GetReviewsByVariantAsync(variant);
                await ResponseHandler.SendSuccess(Response, reviews, 200, "Reviews fetched successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            try
            {
                var deleted = await _service.DeleteReviewAsync(ObjectId.Parse(id));
                await ResponseHandler.SendSuccess(Response, deleted, 200, "Review deleted successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
} 