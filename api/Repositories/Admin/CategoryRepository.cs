using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.Repositories;
using api.models;
using MongoDB.Bson;

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
    }

}