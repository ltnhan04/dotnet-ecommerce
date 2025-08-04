using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.Admin
{
    [ApiController]
    [Route("v1/admin/products/variant")]
    [Authorize(Roles = "admin")]
    public class ProductVariantController : ControllerBase
    {
        private readonly IProductVariantService _productVariantService;

        public ProductVariantController(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }
        [HttpGet("{variantId}")]
        public async Task GetVariantById(string variantId)
        {
            try
            {
                var variant = await _productVariantService.GetVariantById(variantId);
                await ResponseHandler.SendSuccess(Response, variant, 200, "Get variant by id successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPost]
        public async Task CreateVariant([FromForm] CreateProductVariantDto dto)
        {
            try
            {
                var created = await _productVariantService.CreateVariant(dto);
                await ResponseHandler.SendSuccess(Response, created, 200, "Create product successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPut("{variantId}")]
        public async Task UpdateVariant(string variantId, [FromForm] UpdateProductVariantDto dto)
        {
            try
            {
                dto.variantId = variantId;
                var updated = await _productVariantService.UpdateVariant(dto);
                await ResponseHandler.SendSuccess(Response, updated, 200, "Product variant updated successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpDelete("{variantId}")]
        public async Task DeleteVariant(string variantId)
        {
            try
            {
                var result = await _productVariantService.DeleteVariant(variantId);
                await ResponseHandler.SendSuccess(Response, null, 200, result);
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}