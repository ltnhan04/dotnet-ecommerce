using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.models;
using api.Utils;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using ZstdSharp;

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
        public async Task<ProductVariant?> GetVariantById(string variantId)
        {
            var objectId = ObjectId.Parse(variantId);
            return await _context.ProductVariants.Where(v => v._id == objectId).FirstOrDefaultAsync();
        }
        public async Task<ProductVariant> CreateVariant(ProductVariant dto)
        {
            _context.ProductVariants.Add(dto);
            await _context.SaveChangesAsync();
            return dto;
        }
        public async Task<ProductVariant?> UpdateVariant(string id, ProductVariant dto)
        {
            var variant = await GetVariantById(id) ?? throw new AppException("Cannot find variant");
            variant.updatedAt = DateTime.UtcNow;
            _context.Update(variant);
            await _context.SaveChangesAsync();
            return variant;
        }
        public async Task<bool> Delete(string id)
        {
            var variant = await GetVariantById(id);
            if (variant == null) return false;
            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}