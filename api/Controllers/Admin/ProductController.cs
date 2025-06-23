using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.Services;
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
    }
}