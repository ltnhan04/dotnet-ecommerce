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
    public class Create : PageModel
    {
        private readonly ILogger<Create> _logger;
        [BindProperty]
        public CreateProductVariantDto NewVariant { get; set; } = new();
        private readonly HttpClient _httpClient;
        public string? ProductName { get; set; }

        public Create(IHttpClientFactory factory, ILogger<Create> logger)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
            _logger = logger;
        }


        public async Task OnGetAsync(string? productId)
        {
            if (!string.IsNullOrEmpty(productId))
            {
                NewVariant.product = productId;
                var token = Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                var response = await _httpClient.GetAsync($"api/v1/admin/products/{productId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonDocument.Parse(json);
                    if (result.RootElement.TryGetProperty("data", out var data))
                    {
                        ProductName = data.GetProperty("name").GetString();
                    }
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var form = new MultipartFormDataContent
            {
                { new StringContent(NewVariant.product ?? ""), "product" },
                { new StringContent(NewVariant.colorName ?? ""), "colorName" },
                { new StringContent(NewVariant.colorCode ?? ""), "colorCode" },
                { new StringContent(NewVariant.storage ?? ""), "storage" },
                { new StringContent(NewVariant.price.ToString()), "price" },
                { new StringContent(NewVariant.stock_quantity.ToString()), "stock_quantity" },
                { new StringContent(NewVariant.slug ?? ""), "slug" }
            };
            if (Request.Form.Files != null && Request.Form.Files.Count > 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    var streamContent = new StreamContent(file.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    form.Add(streamContent, "images", file.FileName);
                }
            }
            var response = await _httpClient.PostAsync($"api/v1/admin/products/variant", form);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"Tạo variant thất bại: {responseContent}";
                return Page();
            }
            TempData["SuccessMessage"] = "Tạo variant thành công!";
            if (!string.IsNullOrEmpty(NewVariant.product))
                return Redirect($"/Admin/Products/Details/{NewVariant.product}");
            return RedirectToPage("/Admin/Products/Index");
        }
    }
}