using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;
using api.Dtos;


namespace api.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetCategoryById(string categoryId);
        Task<Category> GetSubCategoryById(string subCategoryId);
        Task<List<CategoryDto>> GetAllCategories();
        Task<Category> Create(Category dto);
        Task<Category?> Update(string categoryId, Category dto);
        Task<bool> Delete(string categoryId);
    }
}