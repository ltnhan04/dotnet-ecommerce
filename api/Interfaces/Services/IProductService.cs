using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;

namespace api.Interfaces.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProducts(int page = 1, int size = 10);
        Task<ProductDto> GetProductById(string id);
        Task<List<ProductDto>> GetProductByCategory(string categoryId);
        Task<ProductDto> CreateProduct(CreateProductDto dto);
        Task<ProductDto> UpdateProduct(string id, UpdateProductDto dto);
        Task<string> DeleteProduct(string id);
        Task<PagedResult<ProductDto>> SearchProducts(string? search, string? categoryId, int page = 1, int size = 10, int? minPrice = null, int? maxPrice = null, string? color = null, string? storage = null, string? status = null, int? rating = null);
    }
}