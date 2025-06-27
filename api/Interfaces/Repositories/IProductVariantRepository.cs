using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;
using MongoDB.Bson;

namespace api.Interfaces
{
    public interface IProductVariantRepository
    {
        Task<List<ProductVariant>> GetByProductId(ObjectId productId);
        Task<ProductVariant?> GetVariantById(string variantId);
        Task<ProductVariant> CreateVariant(ProductVariant dto);
        Task<ProductVariant?> UpdateVariant(string id, ProductVariant dto);
        Task<bool> Delete(string id);
    }
}