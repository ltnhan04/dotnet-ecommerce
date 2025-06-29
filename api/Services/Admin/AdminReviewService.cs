using api.Dtos;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.models;
using api.Utils;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Services.Admin
{
    public class AdminReviewService : IAdminReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        public AdminReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllReviews();
            return reviews.Select(r => new ReviewDto
            {
                _id = r._id.ToString(),
                variant = r.variant.ToString(),
                rating = r.rating,
                comment = r.comment
            }).ToList();
        }

        public async Task<ReviewDto> GetReviewByIdAsync(ObjectId id)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null) throw new AppException("Review not found");
            return new ReviewDto
            {
                _id = review._id.ToString(),
                variant = review.variant.ToString(),
                rating = review.rating,
                comment = review.comment
            };
        }

        public async Task<List<ReviewDto>> GetReviewsByVariantIdAsync(ObjectId variantId)
        {
            var reviews = await _reviewRepository.GetReviewByVariantId(variantId);
            return reviews.Select(r => new ReviewDto
            {
                _id = r._id.ToString(),
                variant = r.variant.ToString(),
                rating = r.rating,
                comment = r.comment
            }).ToList();
        }

        public async Task<ReviewDto> DeleteReviewAsync(ObjectId id)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null) throw new AppException("Review not found");
            await _reviewRepository.DeleteReviewAsync(id);
            return new ReviewDto
            {
                _id = review._id.ToString(),
                variant = review.variant.ToString(),
                rating = review.rating,
                comment = review.comment
            };
        }

        public async Task<List<ReviewDto>> GetReviewsByVariantAsync(string variant)
        {
            var reviews = await _reviewRepository.GetReviewsByVariant(variant);
            return reviews.Select(r => new ReviewDto
            {
                _id = r._id.ToString(),
                variant = r.variant.ToString(),
                rating = r.rating,
                comment = r.comment
            }).ToList();
        }
    }
} 