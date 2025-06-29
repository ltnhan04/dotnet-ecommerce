using MongoDB.Bson;
using api.models;
using api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Repositories.Customer
{
    public class CustomerReviewRepository : IReviewRepository
    {
        private readonly iTribeDbContext _context;

        public CustomerReviewRepository(iTribeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetReviewByVariantId(ObjectId variantId)
        {
            return await _context.Reviews
                .Where(r => r.variant == variantId)
                .OrderByDescending(r => r.createdAt)
                .ToListAsync();
        }

        public async Task<Review> GetReviewByIdAsync(ObjectId id)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r._id == id);
        }

        public async Task CreateReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(ObjectId id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r._id == id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Review>> GetReviewsByProductId(string productId)
        {
            return await _context.Reviews
                .Where(r => r.variant.ToString() == productId)
                .OrderByDescending(r => r.createdAt)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByUserId(string userId)
        {
            return await _context.Reviews
                .Where(r => r.user.ToString() == userId)
                .OrderByDescending(r => r.createdAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingByProductId(string productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.variant.ToString() == productId)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.rating);
        }
    }
}