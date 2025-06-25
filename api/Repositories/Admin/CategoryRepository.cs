using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.Repositories;
using api.models;
using MongoDB.Bson;
using Microsoft.EntityFrameworkCore;
using api.Dtos;

namespace api.Repositories.Admin
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly iTribeDbContext _context;
        public CategoryRepository(iTribeDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetCategoryById(string id)
        {
            return await _context.Categories.FindAsync(ObjectId.Parse(id));
        }
        public async Task<Category?> GetSubCategoryById(string subCategoryId)
        {
            return await _context.Categories.FindAsync(ObjectId.Parse(subCategoryId));
        }
        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories.Select(c => new CategoryDto
            {
                name = c.name,
                parent_category = c.parent_category?.ToString()
            }).ToList();
        }
        public async Task<Category> Create(Category dto)
        {
            _context.Categories.Add(dto);
            await _context.SaveChangesAsync();
            return dto;
        }
        public async Task<Category?> Update(string categoryId, Category dto)
        {
            var category = await GetCategoryById(categoryId);
            if (category == null) return null;

            category.name = dto.name;
            category.parent_category = dto.parent_category;
            category.updatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return category;
        }
        public async Task<bool> Delete(string categoryId)
        {
            var category = await GetCategoryById(categoryId);
            if (category == null) return false;
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}