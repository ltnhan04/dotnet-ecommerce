using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.models;
using api.Utils;
using MongoDB.Bson;
using ZstdSharp;

namespace api.Services.Admin
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IProductVariantRepository _variantRepo;
        private readonly ICategoryRepository _categoryRepo;

        public ProductService(
            IProductRepository productRepo,
            IProductVariantRepository variantRepo,
            ICategoryRepository categoryRepo
            )
        {
            _productRepo = productRepo;
            _variantRepo = variantRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<List<ProductDto>> GetAllProducts()
        {
            var products = await _productRepo.GetProducts();

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
        public async Task<ProductDto> GetProductById(string id)
        {
            var product = await _productRepo.GetProductById(id) ?? throw new AppException("Product not found", 404);
            Console.WriteLine("Product: " + product);
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
            return new ProductDto
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
            };
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

        public async Task<ProductDto> CreateProduct(CreateProductDto dto)
        {
            var product = new models.Product
            {
                name = dto.name,
                description = dto.description,
                category = ObjectId.Parse(dto.category),
                createdAt = DateTime.UtcNow,
            };

            var newProduct = await _productRepo.Create(product);
            return new ProductDto
            {
                _id = newProduct._id.ToString(),
                name = newProduct.name,
                description = newProduct.description,
            };
        }
        public async Task<ProductDto> UpdateProduct(string id, UpdateProductDto dto)
        {
            var updates = new models.Product
            {
                name = dto.name,
                description = dto.description,
                category = ObjectId.Parse(dto.category),
            };

            var updatedProduct = await _productRepo.Update(id, updates);
            return new ProductDto
            {
                _id = updatedProduct._id.ToString(),
                name = updatedProduct.name,
                description = updatedProduct.description,
            };
        }
        public async Task<string> DeleteProduct(string id)
        {
            var product = await _productRepo.GetProductById(id) ?? throw new AppException("Product not found", 404);
            _productRepo.DeleteVariants(product._id.ToString());
            var deleted = await _productRepo.Delete(product._id.ToString());
            if (!deleted)
            {
                throw new AppException("Delete product failed");
            }
            var msg = "Product and its variants deleted successfully";
            return msg;
        }
    }
}