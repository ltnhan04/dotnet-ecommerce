// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using api.Dtos;
// using api.Interfaces;
// using api.Interfaces.Repositories;

// namespace api.Services.Admin
// {
//     public class ProductService
//     {
//         private readonly IProductRepository _productRepo;
//         private readonly IProductVariantRepository _variantRepo;
//         private readonly IReviewRepository _reviewRepo;
//         private readonly ICategoryRepository _categoryRepo;

//         public ProductService(
//             IProductRepository productRepo,
//             IProductVariantRepository variantRepo,
//             IReviewRepository reviewRepo,
//             ICategoryRepository categoryRepo
//             )
//         {
//             _productRepo = productRepo;
//             _variantRepo = variantRepo;
//             _reviewRepo = reviewRepo;
//             _categoryRepo = categoryRepo;
//         }

//         public async Task<List<ProductDto>> GetAllProduct()
//         {
//             var products = await _productRepo.GetProducts();
//             var result = new List<ProductDto>();

//             foreach (var product in products)
//             {
//                 var category = await _categoryRepo.GetCategoryById(product.category.ToString());
//                 var variants = await _variantRepo.GetByProductId(product._id);

//                 var variantsDtos = new List

//                 result.Add(new ProductDto
//                 {
//                     _id = product._id.ToString(),
//                     name = product.name,
//                     description = product.description,
//                     category = product.category,
//                      = firstVariant?.price ?? 0,
//                     Stock = firstVariant?.stock_quantity ?? 0,
//                     Rating = firstVariant?.rating ?? 0,
//                     Image = firstVariant?.images.FirstOrDefault()
//                 });
//             }

//             return result;
//         }

//         public async Task<List<Product>> GetByCategoryAsync(string categoryId)
//         {
//             var products = await _productRepo.GetByCategoryAsync(categoryId);
//             if (!products.Any()) throw new Exception("No products found");
//             return products;
//         }

//         public async Task<Product> CreateAsync(CreateProductDto dto)
//         {
//             var product = new Product
//             {
//                 name = dto.name,
//                 description = dto.description,
//                 category = dto.category,
//             };

//             return await _productRepo.CreateAsync(product);
//         }
//     }
// }