using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using api.Services.Admin;
using api.Interfaces.Repositories;
using api.models;
using api.Interfaces.Services;


namespace api.Controllers.Admin
{
    [Route("api/v1/admin/categories")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _logger = logger;
            _categoryService = categoryService;
        }
        [HttpGet("get-all")]
        public async Task GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                await ResponseHandler.SendSuccess(Response, categories, 200, "Get categories successfully!");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [HttpPost("create")]
        public async Task CreateCategory([FromBody] Dtos.CreateCategoryDto categoryDto)
        {
            try
            {
                if (categoryDto == null || string.IsNullOrEmpty(categoryDto.name))
                {
                    await ResponseHandler.SendError(Response, "Invalid category data", 400);
                    return;
                }
                var newCategory = await _categoryService.CreateCategory(categoryDto);
                await ResponseHandler.SendSuccess(Response, newCategory, 201, "Category created successfully!");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPut("update/{categoryId}")]
        public async Task UpdateCategory(string categoryId, [FromBody] Dtos.CreateCategoryDto categoryDto)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryId) || categoryDto == null || string.IsNullOrEmpty(categoryDto.name))
                {
                    await ResponseHandler.SendError(Response, "Invalid category data", 400);
                    return;
                }
                var updatedCategory = await _categoryService.UpdateCategory(categoryId, categoryDto);
                if (updatedCategory == null)
                {
                    await ResponseHandler.SendError(Response, "Category not found", 404);
                    return;
                }
                await ResponseHandler.SendSuccess(Response, updatedCategory, 200, "Category updated successfully!");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [HttpDelete("delete/{categoryId}")]
        public async Task DeleteCategory(string categoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryId))
                {
                    await ResponseHandler.SendError(Response, "Invalid category ID", 400);
                    return;
                }
                var isDeleted = await _categoryService.DeleteCategory(categoryId);
                if (!isDeleted)
                {
                    await ResponseHandler.SendError(Response, "Category not found", 404);
                    return;
                }
                await ResponseHandler.SendSuccess(Response, null, 200, "Category deleted successfully!");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [HttpGet("get-subCategory/{subCategoryId}")]
        public async Task GetSubCategoryById(string subCategoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(subCategoryId))
                {
                    await ResponseHandler.SendError(Response, "Invalid sub-category ID", 400);
                    return;
                }
                var subCategory = await _categoryService.GetSubCategoryById(subCategoryId);
                if (subCategory == null)
                {
                    await ResponseHandler.SendError(Response, "Sub-category not found", 404);
                    return;
                }
                await ResponseHandler.SendSuccess(Response, subCategory, 200, "Get sub-category successfully!");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}