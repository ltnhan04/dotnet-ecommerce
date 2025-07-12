using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using api.Dtos;
using Microsoft.AspNetCore.Authorization;


namespace api.Pages.Admin.Categories
{
    [Authorize(Roles = "admin")]
    public class Delete : PageModel
    {
        private readonly ILogger<Delete> _logger;
        private readonly HttpClient _httpClient;
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Name { get; set; } = string.Empty;

        public Delete(ILogger<Delete> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public Task OnGetAsync() // Thay đổi sang Task vì không còn gọi API ở đây nữa
        {
            // Không cần gọi API để lấy tên danh mục nữa
            // Name đã được tự động bind từ query string nhờ [BindProperty(SupportsGet = true)]

            if (string.IsNullOrEmpty(Id))
            {
                // Có thể thêm log hoặc xử lý lỗi nếu Id không có
                _logger.LogWarning("Delete page accessed without an Id.");
            }
            if (string.IsNullOrEmpty(Name))
            {
                // Có thể log hoặc hiển thị thông báo nếu tên không được truyền
                _logger.LogWarning("Delete page accessed without a Name for Id: {Id}.", Id);
            }

            return Task.CompletedTask; // Trả về Task hoàn thành vì không có thao tác async
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Id)) return Page();
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.DeleteAsync($"api/v1/admin/categories/{Id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            ModelState.AddModelError(string.Empty, "Delete failed.");
            return Page();
        }
    }
}