using api.Dtos;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces.Services
{
    public interface IReviewService
    {
        Task<List<ReviewDto>> GetReviewsByVariantIdAsync(ObjectId variantId);
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto reviewDto, ObjectId userId);
        Task<ReviewDto> UpdateReviewAsync(ObjectId id, ObjectId userId, UpdateReviewDto reviewDto);
        Task<ReviewDto> DeleteReviewAsync(ObjectId id, ObjectId userId);
    }
}