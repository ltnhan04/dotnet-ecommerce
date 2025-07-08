using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using api.Dtos;

namespace api.Pages.Admin.Categories
{
    public class Delete : PageModel
    {
        private readonly ILogger<Delete> _logger;
        private readonly HttpClient _httpClient;
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public string Name { get; set; }

        public Delete(ILogger<Delete> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public async Task OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id)) return;
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            // Gọi endpoint lấy tất cả danh mục cha
            var response = await _httpClient.GetAsync("api/v1/admin/categories");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = System.Text.Json.JsonDocument.Parse(json);
                var data = doc.RootElement.GetProperty("data");
                var list = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDto>>(data.GetRawText()) ?? new();
                var cat = list.FirstOrDefault(c => c._id == Id);
                if (cat != null)
                {
                    Name = cat.name;
                    return;
                }
            }
            // Nếu không tìm thấy ở danh mục cha, thử lấy theo parent (danh mục con)
            // Giả sử Id là parentId, gọi endpoint sub/{parentId}
            var subResponse = await _httpClient.GetAsync($"api/v1/admin/categories/sub/{Id}");
            if (subResponse.IsSuccessStatusCode)
            {
                var json = await subResponse.Content.ReadAsStringAsync();
                var doc = System.Text.Json.JsonDocument.Parse(json);
                var data = doc.RootElement.GetProperty("data");
                var subList = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDto>>(data.GetRawText()) ?? new();
                var subCat = subList.FirstOrDefault(c => c._id == Id);
                if (subCat != null)
                {
                    Name = subCat.name;
                }
            }
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