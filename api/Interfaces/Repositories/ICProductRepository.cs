using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;

namespace api.Interfaces.Repositories
{
    public interface ICProductRepository
    {
        Task<ProductDto> GetProductBySlug(string slug);
    }
}