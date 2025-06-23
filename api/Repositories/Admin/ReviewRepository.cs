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

        public async Task<List<Review>> GetByVariantId(ObjectId variantId)
        {
            return await _context.Reviews.Where(r => r.variant == variantId).ToListAsync();
        }
    }
}