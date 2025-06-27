using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.Utils;

namespace api.Services.Customer
{
    public class CProductService : ICProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IProductVariantRepository _variantRepo;
        private readonly ICProductRepository _cProductRepo;

        public CProductService(IProductRepository productRepo, ICategoryRepository categoryRepo, IProductVariantRepository variantRepo, ICProductRepository cProductRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _variantRepo = variantRepo;
            _cProductRepo = cProductRepo;
        }
        public async Task<List<ProductDto>> GetAllProducts(int page = 1, int size = 10)
        {
            var products = (await _productRepo.GetProducts()).Skip((page - 1) * size).Take(size).ToList();

            var result = new List<ProductDto>();

            foreach (var product in products)
            {
                var category = await _categoryRepo.GetCategoryById(product.category.ToString());
                var variants = await _variantRepo.GetByProductId(product._id);
                var variantsDtos = new List<VariantDto>();

                foreach (var variant in variants)
                {
                    variantsDtos.Add(new VariantDto
                    {
                        _id = variant._id.ToString(),
                        product = variant.product.ToString(),
                        color = new ColorDto
                        {
                            colorName = variant.color.colorName,
                            colorCode = variant.color.colorCode
                        },
                        rating = (int)Math.Round(variant.rating),
                        storage = variant.storage,
                        price = variant.price,
                        status = variant.status,
                        stock_quantity = variant.stock_quantity,
                        slug = variant.slug,
                        images = variant.images,
                    });
                }
                result.Add(new ProductDto
                {
                    _id = product._id.ToString(),
                    name = product.name,
                    description = product.description,
                    category = new CategoryDto
                    {
                        _id = category._id.ToString(),
                        name = category.name
                    },
                    variants = variantsDtos,
                    createdAt = product.createdAt,
                    updatedAt = product.updatedAt
                });
            }
            return result;
        }
        public async Task<List<ProductDto>> GetProductByCategory(string categoryId)
        {
            var products = await _productRepo.GetByCategory(categoryId) ?? throw new AppException("No products found", 400);
            var result = new List<ProductDto>();
            foreach (var product in products)
            {
                var category = await _categoryRepo.GetCategoryById(product.category.ToString());
                var variants = await _variantRepo.GetByProductId(product._id);

                var variantsDtos = new List<VariantDto>();

                foreach (var variant in variants)
                {
                    variantsDtos.Add(new VariantDto
                    {
                        _id = variant._id.ToString(),
                        product = variant.product.ToString(),
                        color = new ColorDto
                        {
                            colorName = variant.color.colorName,
                            colorCode = variant.color.colorCode
                        },
                        rating = (int)Math.Round(variant.rating),
                        storage = variant.storage,
                        price = variant.price,
                        stock_quantity = variant.stock_quantity,
                        slug = variant.slug,
                        images = variant.images,
                    });
                }
                result.Add(new ProductDto
                {
                    _id = product._id.ToString(),
                    name = product.name,
                    description = product.description,
                    category = new CategoryDto
                    {
                        _id = category?._id.ToString() ?? string.Empty,
                        name = category?.name ?? string.Empty
                    },
                    variants = variantsDtos,
                    createdAt = product.createdAt,
                    updatedAt = product.updatedAt
                });
            }
            return result;
        }
        public async Task<ProductDto> GetProductBySlug(string slug)
        {
            return await _cProductRepo.GetProductBySlug(slug);
        }
    }
}