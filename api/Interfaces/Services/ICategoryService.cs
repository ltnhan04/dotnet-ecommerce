using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;

namespace api.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategories();
        Task<CreateCategoryDto> CreateCategory(CreateCategoryDto categoryDto);
        Task<CategoryDto> UpdateCategory(string categoryId, CreateCategoryDto categoryDto);
        Task<bool> DeleteCategory(string categoryId);
    }
}