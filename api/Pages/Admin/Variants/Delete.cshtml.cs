using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using api.Dtos;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace api.Pages.Admin.Variants
{
    public class Delete : PageModel
    {
        private readonly ILogger<Delete> _logger;
        public string? ProductId { get; set; }
        public string? VariantId { get; set; }
        private readonly HttpClient _httpClient;

        public Delete(IHttpClientFactory factory, ILogger<Delete> logger)
        {
            _logger = logger;
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public async Task OnGetAsync(string id)
        {
            VariantId = id;
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetAsync($"v1/admin/products/variant/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(json);
                if (result.RootElement.TryGetProperty("data", out var data))
                {
                    var variant = JsonSerializer.Deserialize<ProductVariantDto>(data.ToString());
                    ProductId = variant?.product;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.DeleteAsync($"v1/admin/products/variant/{id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"Xóa variant thất bại: {responseContent}";
                return Page();
            }
            TempData["SuccessMessage"] = "Xóa variant thành công!";
            if (!string.IsNullOrEmpty(ProductId))
                return Redirect($"/Admin/Products/Details/{ProductId}");
            return RedirectToPage("/Admin/Products/Index");
        }
    }
}