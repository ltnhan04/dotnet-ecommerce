using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;

namespace api.Interfaces.Services
{
    public interface ICProductService
    {
        Task<List<ProductDto>> GetAllProducts(int page = 1, int size = 10);
        Task<List<ProductDto>> GetProductByCategory(string categoryId);
        Task<ProductDto> GetProductBySlug(string slug);
    }
}