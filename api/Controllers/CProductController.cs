using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Services;
using api.Services.Customer;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class CProductController : ControllerBase
    {
        private readonly ICProductService _cProductService;
        public CProductController(ICProductService cProductService)
        {
            _cProductService = cProductService;
        }
        [HttpGet("filter")]
        public async Task GetProductByCategory([FromQuery] string categoryId)
        {
            try
            {
                var product = await _cProductService.GetProductByCategory(categoryId);
                await ResponseHandler.SendSuccess(Response, product, 200, "Get product successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [HttpGet]
        public async Task GetProducts()
        {
            try
            {
                var products = await _cProductService.GetAllProducts();
                await ResponseHandler.SendSuccess(Response, products, 200, "Get products successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [HttpPost]
        public async Task GetProductBySlug(ProductSlug dto)
        {
            try
            {
                var products = await _cProductService.GetProductBySlug(dto.slug);
                await ResponseHandler.SendSuccess(Response, products, 200, "Get products successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}