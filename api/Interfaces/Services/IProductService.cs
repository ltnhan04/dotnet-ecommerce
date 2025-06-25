using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;

namespace api.Interfaces.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProducts();
        Task<ProductDto> GetProductById(string id);
        Task<List<ProductDto>> GetProductByCategory(string categoryId);
        Task<ProductDto> CreateProduct(CreateProductDto dto);
        Task<ProductDto> UpdateProduct(string id, UpdateProductDto dto);
        Task<string> DeleteProduct(string id);
    }
}