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
        [HttpGet("")]
        public async Task GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProducts();
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
        [HttpPost("")]
        public async Task CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                var product = _productService.CreateProduct(dto) ?? throw new AppException("Create product failed");
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
                var updatedProduct = _productService.UpdateProduct(id, dto) ?? throw new AppException("Update product failed");
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
                var updatedProduct = _productService.DeleteProduct(id) ?? throw new AppException("Delete product failed");
                await ResponseHandler.SendSuccess(Response, updatedProduct, 200, "Deleted Product Successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}