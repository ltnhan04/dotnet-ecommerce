using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;

namespace api.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetCategoryById(string categoryId);
    }
}