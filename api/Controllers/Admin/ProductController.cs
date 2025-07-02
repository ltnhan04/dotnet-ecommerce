using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Services;
using api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Roles = "admin")]

        [HttpGet]
        public async Task GetAllProducts([FromQuery] int size, int page)
        {
            try
            {
                var products = await _productService.GetAllProducts(page, size);
                await ResponseHandler.SendSuccess(Response, products, 200, "Get products successfully!");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{productId}")]
        public async Task GetProductById(string productId)
        {
            try
            {
                var product = await _productService.GetProductById(productId);
                await ResponseHandler.SendSuccess(Response, product, 200, "Get product successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [Authorize(Roles = "admin")]
        [HttpGet("filter")]
        public async Task GetProductByCategory([FromQuery] string categoryId)
        {
            try
            {
                var products = await _productService.GetProductByCategory(categoryId);
                await ResponseHandler.SendSuccess(Response, products, 200, "Get product by category successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                var product = await _productService.CreateProduct(dto);
                await ResponseHandler.SendSuccess(Response, product, 200, "Created Product Successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task UpdateProduct([FromBody] UpdateProductDto dto, string id)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProduct(id, dto) ?? throw new AppException("Update product failed");
                await ResponseHandler.SendSuccess(Response, updatedProduct, 200, "Updated Product Successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task DeleteProduct(string id)
        {
            try
            {
                var deletedProduct = await _productService.DeleteProduct(id) ?? throw new AppException("Delete product failed");
                await ResponseHandler.SendSuccess(Response, null, 200, "Deleted Product Successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("search")]
        public async Task SearchProducts(
            [FromQuery] string? search,
            [FromQuery] string? categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] int? minPrice = null,
            [FromQuery] int? maxPrice = null,
            [FromQuery] string? color = null,
            [FromQuery] string? storage = null,
            [FromQuery] string? status = null,
            [FromQuery] int? rating = null)
        {
            try
            {
                var result = await _productService.SearchProducts(search, categoryId, page, size, minPrice, maxPrice, color, storage, status, rating);
                await ResponseHandler.SendSuccess(Response, result, 200, "Search products successfully!");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}