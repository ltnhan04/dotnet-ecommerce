using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace api.Repositories.Admin
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly iTribeDbContext _context;
        public ProductVariantRepository(iTribeDbContext context)
        {
            _context = context;
        }
        public async Task<List<ProductVariant>> GetByProductId(ObjectId productId)
        {
            return await _context.ProductVariants.Where(v => v.product == productId).ToListAsync();
        }
    }
}