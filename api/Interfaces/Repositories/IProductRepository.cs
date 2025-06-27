using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using Product = api.models.Product;

namespace api.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProducts();
        Task<List<Product>> GetByCategory(string categoryId);
        Task<Product?> GetProductById(string productId);
        Task<Product> Create(Product dto);
        Task<Product> Update(string productId, Product dto);
        Task<bool> Delete(string productId);
        void DeleteVariants(string productId);
    }
}