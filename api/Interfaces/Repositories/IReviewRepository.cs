using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;
using MongoDB.Bson;

namespace api.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetReviewByVariantId(ObjectId variantId);
    }
}