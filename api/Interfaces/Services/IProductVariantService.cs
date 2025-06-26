using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;

namespace api.Interfaces.Services
{
    public interface IProductVariantService
    {
        Task<ProductVariantDto> CreateVariant(CreateProductVariantDto dto);
        Task<ProductVariantDto> UpdateVariant(UpdateProductVariantDto dto);
        Task<ProductVariantDto> GetVariantById(string variantId);
        Task<string> DeleteVariant(string id);
    }
}