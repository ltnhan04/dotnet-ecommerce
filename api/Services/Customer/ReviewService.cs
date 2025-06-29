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

namespace api.Services.Customer
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<ReviewDto>> GetReviewsByVariantIdAsync(ObjectId variantId)
        {
            var reviews = await _reviewRepository.GetReviewByVariantId(variantId);
            return reviews.Select(r => new ReviewDto
            {
                variant = r.variant.ToString(),
                rating = r.rating,
                comment = r.comment
            }).ToList();
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto reviewDto, ObjectId userId)
        {
            if (!ObjectId.TryParse(reviewDto.variant, out var variantId))
            {
                throw new AppException("Invalid variant ID format");
            }

            var review = new Review
            {
                variant = variantId,
                user = userId,
                rating = reviewDto.rating,
                comment = reviewDto.comment,
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            };

            await _reviewRepository.CreateReviewAsync(review);

            return new ReviewDto
            {
                _id = review._id.ToString(),
                variant = review.variant.ToString(),
                rating = review.rating,
                comment = review.comment
            };
        }

        public async Task<ReviewDto> UpdateReviewAsync(ObjectId id, ObjectId userId, UpdateReviewDto reviewDto)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null || review.user != userId)
            {
                throw new AppException("Review not found or user not authorized");
            }

            review.rating = reviewDto.rating;
            review.comment = reviewDto.comment;
            review.updatedAt = DateTime.UtcNow;

            await _reviewRepository.UpdateReviewAsync(review);

            return new ReviewDto
            {
                _id = review._id.ToString(),
                variant = review.variant.ToString(),
                rating = review.rating,
                comment = review.comment
            };
        }

        public async Task<ReviewDto> DeleteReviewAsync(ObjectId id, ObjectId userId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null || review.user != userId)
            {
                throw new AppException("Review not found or user not authorized");
            }

            await _reviewRepository.DeleteReviewAsync(id);

            return new ReviewDto
            {
                _id = review._id.ToString(),
                variant = review.variant.ToString(),
                rating = review.rating,
                comment = review.comment
            };
        }
    }
}