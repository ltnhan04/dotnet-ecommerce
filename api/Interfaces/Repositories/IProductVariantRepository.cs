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
    }
}