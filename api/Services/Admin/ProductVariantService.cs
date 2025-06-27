using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Interfaces.Services;
using api.models;
using api.Utils;
using CloudinaryDotNet;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace api.Services.Admin
{
    public class ProductVariantService(IProductVariantRepository productVariantRepository, CloudinaryUtils cloudinaryUtils, IProductRepository productRepository) : IProductVariantService
    {
        private readonly IProductVariantRepository _productVariantRepository = productVariantRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly CloudinaryUtils _cloudinaryUtils = cloudinaryUtils;

        public async Task<ProductVariantDto> GetVariantById(string variantId)
        {
            var variant = await _productVariantRepository.GetVariantById(variantId) ?? throw new AppException("Variant not found");
            return new ProductVariantDto
            {
                _id = variant._id.ToString(),
                product = variant.product.ToString(),
                price = variant.price,
                storage = variant.storage,
                stock_quantity = variant.stock_quantity,
                images = variant.images,
                slug = variant.slug,
                colorCode = variant.color.colorCode,
                colorName = variant.color.colorName,
                createdAt = variant.createdAt,
                updatedAt = variant.updatedAt
            };
        }
        public async Task<ProductVariantDto> CreateVariant(CreateProductVariantDto dto)
        {
            var product = await _productRepository.GetProductById(dto.product) ?? throw new AppException("Product not found");
            var imageUrls = await _cloudinaryUtils.UploadImage(dto.images);

            var variant = new ProductVariant
            {
                product = ObjectId.Parse(dto.product),
                color = new models.Color { colorName = dto.colorName, colorCode = dto.colorCode },
                storage = dto.storage,
                price = dto.price,
                stock_quantity = dto.stock_quantity,
                slug = dto.slug,
                images = imageUrls,
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            };

            var created = await _productVariantRepository.CreateVariant(variant);
            product.variants.Add(created._id);
            await _productRepository.Update(product._id.ToString(), product);

            return new ProductVariantDto
            {
                _id = created._id.ToString(),
                product = created.product.ToString(),
                colorName = created.color.colorName,
                colorCode = created.color.colorCode,
                storage = created.storage,
                price = created.price,
                stock_quantity = created.stock_quantity,
                slug = created.slug,
                images = created.images,
                createdAt = created.createdAt,
                updatedAt = created.updatedAt
            };
        }
        public async Task<ProductVariantDto> UpdateVariant(UpdateProductVariantDto dto)
        {
            var variant = await _productVariantRepository.GetVariantById(dto.variantId)
                          ?? throw new AppException("Product variant not found");
            var existingImage = dto.existingImages ?? [];
            if (existingImage.Count == 0)
            {
                existingImage = variant.images;
            }
            var uploadedImages = dto.images != null && dto.images.Count > 0 ? await _cloudinaryUtils.UploadImage(dto.images) : [];
            var finalImages = existingImage.Concat(uploadedImages).ToList();


            variant.product = string.IsNullOrWhiteSpace(dto.product) ? variant.product : ObjectId.Parse(dto.product);
            variant.color.colorName = string.IsNullOrWhiteSpace(dto.colorName) ? variant.color.colorName : dto.colorName;
            variant.color.colorCode = string.IsNullOrWhiteSpace(dto.colorCode) ? variant.color.colorCode : dto.colorCode;
            variant.storage = string.IsNullOrWhiteSpace(dto.storage) ? variant.storage : dto.storage;
            variant.price = dto.price != 0 ? dto.price : variant.price;
            variant.stock_quantity = dto.stock_quantity != 0 ? variant.stock_quantity : dto.stock_quantity;
            variant.slug = string.IsNullOrWhiteSpace(dto.slug) ? variant.slug : dto.slug;
            variant.images = finalImages;
            variant.updatedAt = DateTime.UtcNow;

            var updated = await _productVariantRepository.UpdateVariant(dto.variantId, variant);

            return new ProductVariantDto
            {
                _id = updated._id.ToString(),
                product = updated.product.ToString(),
                colorName = updated.color.colorName,
                colorCode = updated.color.colorCode,
                storage = updated.storage,
                price = updated.price,
                stock_quantity = updated.stock_quantity,
                slug = updated.slug,
                images = updated.images,
                createdAt = updated.createdAt,
                updatedAt = updated.updatedAt
            };
        }


        public async Task<string> DeleteVariant(string id)
        {
            var variant = await _productVariantRepository.GetVariantById(id)
                          ?? throw new AppException("Product variant not found");

            var product = await _productRepository.GetProductById(variant.product.ToString())
                          ?? throw new AppException("Product not found");

            if (variant.images != null && variant.images.Count > 0)
            {
                await _cloudinaryUtils.DeleteImage(variant.images);
            }
            var success = await _productVariantRepository.Delete(id);
            if (!success)
                throw new AppException("Delete failed");
            product.variants = [.. product.variants.Where(vid => vid != variant._id)];
            await _productRepository.Update(product._id.ToString(), product);
            return "Product variant deleted successfully!";
        }

    }
}