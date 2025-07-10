using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Repositories;
using api.models;
using api.Utils;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace api.Repositories.Customer
{
    public class CProductRepository : ICProductRepository
    {
        private readonly iTribeDbContext _context;
        public CProductRepository(iTribeDbContext context)
        {
            _context = context;
        }
        public async Task<ProductDto> GetProductBySlug(string slug)
        {
            var variants = await _context.ProductVariants
                .Where(v => v.slug == slug)
                .ToListAsync();

            if (variants == null || variants.Count == 0)
            {
                throw new AppException("No variants found");
            }
            var products = await _context.Products.ToListAsync();
            var product = products.FirstOrDefault(p => p._id == variants[0].product) ?? throw new AppException("Product not found");
            var allVariants = await _context.ProductVariants
                .Where(v => v.product == product._id)
                .ToListAsync();

            var variantDtos = new List<VariantDto>();
            foreach (var variant in allVariants)
            {
                var reviews = await _context.Reviews
                    .Where(r => r.variant == variant._id)
                    .ToListAsync();
                int rating = reviews.Count > 0 ? (int)Math.Round(reviews.Average(r => r.rating)) : 0;
                variantDtos.Add(new VariantDto
                {
                    _id = variant._id.ToString(),
                    product = variant.product.ToString(),
                    color = new ColorDto
                    {
                        colorName = variant.color.colorName,
                        colorCode = variant.color.colorCode
                    },
                    rating = rating,
                    storage = variant.storage,
                    price = variant.price,
                    stock_quantity = variant.stock_quantity,
                    slug = variant.slug,
                    images = variant.images,
                    reviews = [.. reviews.Select(r => new ReviewDto
                    {
                        _id = r._id.ToString(),
                        variant = r.variant.ToString(),
                        rating = r.rating,
                        comment = r.comment ?? ""
                    })]
                });
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c._id == product.category);

            return new ProductDto
            {
                _id = product._id.ToString(),
                name = product.name,
                description = product.description,
                category = new CategoryDto
                {
                    _id = category?._id.ToString() ?? string.Empty,
                    name = category?.name ?? string.Empty,
                    parent_category = category?.parent_category?.ToString()
                },
                variants = variantDtos,
                createdAt = product.createdAt,
                updatedAt = product.updatedAt
            };
        }
    }
}