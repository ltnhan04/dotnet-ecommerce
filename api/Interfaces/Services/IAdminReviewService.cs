using api.Dtos;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces.Services
{
    public interface IAdminReviewService
    {
        Task<List<ReviewDto>> GetAllReviewsAsync();
        Task<ReviewDto> GetReviewByIdAsync(ObjectId id);
        Task<List<ReviewDto>> GetReviewsByVariantIdAsync(ObjectId variantId);
        Task<ReviewDto> DeleteReviewAsync(ObjectId id);
        Task<List<ReviewDto>> GetReviewsByVariantAsync(string variant);
    }
} 