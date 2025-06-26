using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.models;
using api.Utils;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Product = api.models.Product;

namespace api.Repositories.Admin
{
    public class ProductRepository : IProductRepository
    {
        private readonly iTribeDbContext _context;
        public ProductRepository(iTribeDbContext context)
        {
            _context = context;
        }
        public async Task<List<Product>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<List<Product>> GetByCategory(string categoryId)
        {
            return await _context.Products.Where(p => p.category == ObjectId.Parse(categoryId)).ToListAsync();
        }
        public async Task<Product?> GetProductById(string productId)
        {
            return await _context.Products.FindAsync(ObjectId.Parse(productId));
        }
        public async Task<Product> Create(Product dto)
        {
            _context.Products.Add(dto);
            await _context.SaveChangesAsync();
            return dto;
        }
        public async Task<Product> Update(string productId, Product dto)
        {
            var product = await GetProductById(productId) ?? throw new AppException("Product not found", 400);
            product.name = dto.name;
            product.description = dto.description;
            product.category = dto.category;
            product.updatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<bool> Delete(string productId)
        {
            var product = await GetProductById(productId);
            if (product == null) return false;
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        public void DeleteVariants(string productId)
        {
            var relatedVariants = _context.ProductVariants.Where(v => v.product == ObjectId.Parse(productId));
            _context.ProductVariants.RemoveRange(relatedVariants);
            return;
        }
    }
}