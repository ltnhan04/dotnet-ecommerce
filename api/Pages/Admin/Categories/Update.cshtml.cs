using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using api.Dtos; // Đảm bảo namespace này chứa CategoryDto và CreateCategoryDto
using System.Collections.Generic;

namespace api.Pages.Admin.Categories
{
    [Authorize(Roles = "admin")]
    public class Update : PageModel
    {
        private readonly ILogger<Update> _logger;
        private readonly HttpClient _httpClient;

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string? ParentCategoryId { get; set; }

        [BindProperty]
        public CreateCategoryDto Category { get; set; } = new();

        // Đảm bảo đây là List<CategoryDto> để khớp với API Product Edit Model
        public List<CategoryDto> Categories { get; set; } = new();

        public Update(IHttpClientFactory clientFactory, ILogger<Update> logger)
        {
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Authentication token not found. Please log in again.");
                return RedirectToPage("/Account/Login");
            }

            // Populate the 'Category' DTO directly from the URL parameters
            Category = new CreateCategoryDto
            {
                _id = Id,
                name = Name,
                parent_category = ParentCategoryId
            };

            // Gọi hàm helper để lấy danh sách danh mục, giống như cách trong EditModel
            await LoadCategoriesForDropdown();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesForDropdown();
                return Page();
            }

            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Authentication token not found. Please log in again.");
                return RedirectToPage("/Account/Login");
            }

            if (string.IsNullOrWhiteSpace(Category.parent_category))
            {
                Category.parent_category = null;
            }

            var response = await _httpClient.PutAsJsonAsync($"api/v1/admin/categories/{Id}", Category);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Admin/Categories/Index");
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to update category: {Error}", error);

            ModelState.AddModelError(string.Empty, "Cập nhật thất bại. Vui lòng kiểm tra lại thông tin.");
            await LoadCategoriesForDropdown();
            return Page();
        }

        private async Task LoadCategoriesForDropdown()
        {
            var token = Request.Cookies["accessToken"];
            if (string.IsNullOrEmpty(token)) return;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var catRes = await _httpClient.GetAsync("api/v1/admin/categories");
            if (catRes.IsSuccessStatusCode)
            {
                var catJson = await catRes.Content.ReadAsStringAsync();
                // Dòng này được sửa để khớp với EditModel.cshtml.cs
                // Nó giả định API trả về JSON có cấu trúc {"data": [...]}
                Categories = JsonSerializer.Deserialize<List<CategoryDto>>(JsonDocument.Parse(catJson).RootElement.GetProperty("data").ToString()) ?? new();
            }
            else
            {
                _logger.LogError("Failed to reload categories for dropdown: {StatusCode} {Error}", catRes.StatusCode, await catRes.Content.ReadAsStringAsync());
                ModelState.AddModelError(string.Empty, "Không thể tải danh sách danh mục cha.");
            }
        }
    }
}