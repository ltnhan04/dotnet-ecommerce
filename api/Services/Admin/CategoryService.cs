using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.Repositories;
using api.models;
using api.Interfaces.Services;
using api.Dtos;
using MongoDB.Bson;
using api.Utils;


namespace api.Services.Admin
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategories();
            return [.. categories.Select(c => new CategoryDto
            {
                _id = c._id.ToString(),
                name = c.name,
                parent_category = c.parent_category
            })];
        }
        public async Task<CreateCategoryDto> CreateCategory(CreateCategoryDto categoryDto)
        {
            var category = new models.Category
            {
                name = categoryDto.name,
                parent_category = string.IsNullOrEmpty(categoryDto.parent_category) ? null : ObjectId.Parse(categoryDto.parent_category),
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            };
            var newCategory = await _categoryRepository.Create(category);
            return new CreateCategoryDto
            {
                _id = newCategory._id.ToString(),
                name = newCategory.name,
                parent_category = newCategory.parent_category?.ToString()
            };
        }
        public async Task<CategoryDto> UpdateCategory(string categoryId, CreateCategoryDto categoryDto)
        {
            var category = new models.Category
            {
                name = categoryDto.name,
                parent_category = ObjectId.Parse(categoryDto.parent_category ?? string.Empty),
                updatedAt = DateTime.UtcNow,
            };
            var updatedCategory = await _categoryRepository.Update(categoryId, category) ?? throw new AppException("Update category failed");


            return new CategoryDto
            {
                _id = updatedCategory._id.ToString(),
                name = updatedCategory.name,
                parent_category = updatedCategory.parent_category?.ToString()
            };
        }
        public async Task<bool> DeleteCategory(string categoryId)
        {
            return await _categoryRepository.Delete(categoryId);
        }
    }
}