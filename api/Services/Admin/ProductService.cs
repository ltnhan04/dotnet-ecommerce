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
        public async Task<ProductDto> GetProductById(string id)
        {
            var product = await _productRepo.GetProductById(id) ?? throw new AppException("Product not found", 404);
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
                updatedAt = DateTime.UtcNow,
            };
            var newProduct = await _productRepo.Create(product) ?? throw new AppException("Create product failed");
            var category = await _categoryRepo.GetCategoryById(newProduct.category.ToString());
            return new ProductDto
            {
                _id = newProduct._id.ToString(),
                name = newProduct.name,
                description = newProduct.description,
                category = new CategoryDto
                {
                    _id = category._id.ToString(),
                    name = category.name
                }

            };
        }
        public async Task<ProductDto> UpdateProduct(string id, UpdateProductDto dto)
        {
            var existingProduct = await _productRepo.GetProductById(id)
                                    ?? throw new AppException("Product not found", 404);
            var category = await _categoryRepo.GetCategoryById(existingProduct.category.ToString());

            if (!string.IsNullOrWhiteSpace(dto.name))
                existingProduct.name = dto.name;

            if (!string.IsNullOrWhiteSpace(dto.description))
                existingProduct.description = dto.description;

            if (!string.IsNullOrWhiteSpace(dto.category))
            {
                if (dto.category.Length != 24)
                    throw new AppException("Invalid category id", 400);

                existingProduct.category = ObjectId.Parse(dto.category);
            }

            existingProduct.updatedAt = DateTime.UtcNow;

            var updated = await _productRepo.Update(id, existingProduct);

            return new ProductDto
            {
                _id = updated._id.ToString(),
                name = updated.name,
                description = updated.description,
                category = new CategoryDto
                {
                    _id = category._id.ToString(),
                    name = category.name
                }
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

        public async Task<PagedResult<ProductDto>> SearchProducts(string? search, string? categoryId, int page = 1, int size = 10, int? minPrice = null, int? maxPrice = null, string? color = null, string? storage = null, string? status = null, int? rating = null)
        {
            var allProducts = await _productRepo.GetProducts();
            if (!string.IsNullOrEmpty(categoryId))
                allProducts = allProducts.Where(p => p.category.ToString() == categoryId).ToList();
            if (!string.IsNullOrEmpty(search))
                allProducts = allProducts.Where(p => p.name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            var filteredProducts = new List<models.Product>();
            foreach (var product in allProducts)
            {
                var variants = await _variantRepo.GetByProductId(product._id);
                var filteredVariants = variants.Where(v =>
                    (!minPrice.HasValue || v.price >= minPrice.Value)
                    && (!maxPrice.HasValue || v.price <= maxPrice.Value)
                    && (string.IsNullOrEmpty(color) || v.color.colorName == color)
                    && (string.IsNullOrEmpty(storage) || v.storage == storage)
                    && (string.IsNullOrEmpty(status) || v.status == status)
                    && (!rating.HasValue || Math.Round(v.rating) >= rating.Value)
                ).ToList();
                if (filteredVariants.Count != 0)
                {
                    product.variants = filteredVariants.Select(v => v._id).ToList();
                    filteredProducts.Add(product);
                }
            }
            var total = filteredProducts.Count;
            var products = filteredProducts.Skip((page - 1) * size).Take(size).ToList();
            var result = new List<ProductDto>();
            foreach (var product in products)
            {
                var category = await _categoryRepo.GetCategoryById(product.category.ToString());
                var variants = await _variantRepo.GetByProductId(product._id);
                var filteredVariants = variants.Where(v =>
                    (!minPrice.HasValue || v.price >= minPrice.Value)
                    && (!maxPrice.HasValue || v.price <= maxPrice.Value)
                    && (string.IsNullOrEmpty(color) || v.color.colorName == color)
                    && (string.IsNullOrEmpty(storage) || v.storage == storage)
                    && (string.IsNullOrEmpty(status) || v.status == status)
                    && (!rating.HasValue || Math.Round(v.rating) >= rating.Value)
                ).ToList();
                var variantsDtos = new List<VariantDto>();
                foreach (var variant in filteredVariants)
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
            return new PagedResult<ProductDto>
            {
                Items = result,
                Total = total,
                Page = page,
                Size = size
            };
        }
    }
}