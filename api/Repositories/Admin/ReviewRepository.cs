using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.Repositories;
using api.models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace api.Repositories.Admin
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly iTribeDbContext _context;

        public ReviewRepository(iTribeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetReviewByVariantId(ObjectId variantId)
        {
            return await _context.Reviews.Where(r => r.variant == variantId).ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(ObjectId id)
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

        public async Task<List<Review>> GetAllReviews()
        {
            return await _context.Reviews
                .OrderByDescending(r => r.createdAt)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByVariant(string variant)
        {
            return await _context.Reviews
                .Where(r => r.variant.ToString() == variant)
                .OrderByDescending(r => r.createdAt)
                .ToListAsync();
        }
    }
}