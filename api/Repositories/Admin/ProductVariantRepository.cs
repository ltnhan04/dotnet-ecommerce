using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Interfaces.Repositories;
using api.models;
using api.Models;
using api.Utils;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace api.Repositories.Admin
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly iTribeDbContext _context;
        private readonly INotificationRepository _notificationRepository;

        public ProductVariantRepository(iTribeDbContext context, INotificationRepository notificationRepository)
        {
            _context = context;
            _notificationRepository = notificationRepository;
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
        public async Task CheckVariantLowStock(string variantId)
        {
            var variant = await _context.ProductVariants
        .FirstOrDefaultAsync(v => v._id == ObjectId.Parse(variantId));

            if (variant != null && variant.stock_quantity < 10)
            {
                await _notificationRepository.Create(new Notification
                {
                    title = "⚠️ Sản phẩm sắp hết hàng",
                    message = $"Variant {variant.storage} - {variant.color.colorName} chỉ còn {variant.stock_quantity} sản phẩm.",
                    targetRole = "admin",
                    type = "inventory",
                    isRead = false,
                    redirectUrl = "/admin/products",
                    createdAt = DateTime.UtcNow
                });
            }
        }
    }
}