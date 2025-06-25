using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.Repositories;
using api.models;
using api.Interfaces.Services;
using api.Dtos;
using MongoDB.Bson;


namespace api.Services.Admin
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var categories = await categoryRepository.GetAllCategories();
            return categories.Select(c => new CategoryDto
            {
                name = c.name,
                parent_category = c.parent_category
            }).ToList();
        }
        public async Task<CategoryDto?> GetSubCategoryById(string subCategoryId)
        {
            var subCategory = await categoryRepository.GetSubCategoryById(subCategoryId);
            if (subCategory == null) return null;
            return new CategoryDto
            {
                name = subCategory.name,
                parent_category = subCategory.parent_category?.ToString()
            };
        }
        public async Task<CreateCategoryDto> CreateCategory(CreateCategoryDto categoryDto)
        {
            var category = new models.Category
            {
                name = categoryDto.name,
                parent_category = string.IsNullOrEmpty(categoryDto.parent_category) ? null : ObjectId.Parse(categoryDto.parent_category)
            };
            var newCategory = await categoryRepository.Create(category);
            return new CreateCategoryDto
            {
                name = newCategory.name,
                parent_category = newCategory.parent_category?.ToString()
            };
        }
        public async Task<CategoryDto> UpdateCategory(string categoryId, CreateCategoryDto categoryDto)
        {
            var category = new models.Category
            {
                name = categoryDto.name,
                parent_category = ObjectId.Parse(categoryDto.parent_category ?? string.Empty)
            };
            var updatedCategory = await categoryRepository.Update(categoryId, category);
            if (updatedCategory == null) return null;

            return new CategoryDto
            {
                name = updatedCategory.name,
                parent_category = updatedCategory.parent_category?.ToString()
            };
        }
        public async Task<bool> DeleteCategory(string categoryId)
        {
            return await categoryRepository.Delete(categoryId);
        }
    }
}