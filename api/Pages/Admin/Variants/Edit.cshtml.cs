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
    public class Edit : PageModel
    {
        private readonly ILogger<Edit> _logger;
        [BindProperty]
        public UpdateProductVariantDto EditVariant { get; set; } = new();
        private readonly HttpClient _httpClient;
        public string? ProductName { get; set; }

        public class ProductVariantDto
        {
            public string _id { get; set; } = string.Empty;
            public string product { get; set; } = string.Empty;
            public string colorName { get; set; } = string.Empty;
            public string colorCode { get; set; } = string.Empty;
            public string storage { get; set; } = string.Empty;
            public int price { get; set; } = 0;
            public int stock_quantity { get; set; } = 0;
            public string slug { get; set; } = string.Empty;
            public List<string> images { get; set; } = [];
        }

        public Edit(IHttpClientFactory factory, ILogger<Edit> logger)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
            _logger = logger;
        }

        public async Task OnGetAsync(string id)
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetAsync($"api/v1/admin/products/variant/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(json);
                if (result.RootElement.TryGetProperty("data", out var data))
                {
                    var variant = JsonSerializer.Deserialize<ProductVariantDto>(data.ToString());
                    if (variant != null)
                    {
                        EditVariant.variantId = id;
                        EditVariant.product = variant.product;
                        EditVariant.colorName = variant.colorName;
                        EditVariant.colorCode = variant.colorCode;
                        EditVariant.storage = variant.storage;
                        EditVariant.price = variant.price;
                        EditVariant.stock_quantity = variant.stock_quantity;
                        EditVariant.slug = variant.slug;
                        EditVariant.existingImages = variant.images ?? new List<string>();
                        // Lấy product name từ API
                        var productResponse = await _httpClient.GetAsync($"api/v1/admin/products/{variant.product}");
                        if (productResponse.IsSuccessStatusCode)
                        {
                            var productJson = await productResponse.Content.ReadAsStringAsync();
                            var productResult = JsonDocument.Parse(productJson);
                            if (productResult.RootElement.TryGetProperty("data", out var pdata))
                            {
                                ProductName = pdata.GetProperty("name").GetString();
                            }
                        }
                    }
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
            var deleteImages = Request.Form["DeleteImages"].ToList();
            if (EditVariant.existingImages != null && deleteImages.Count > 0)
            {
                EditVariant.existingImages = EditVariant.existingImages.Where(img => !deleteImages.Contains(img)).ToList();
            }
            var form = new MultipartFormDataContent
            {
                { new StringContent(EditVariant.variantId), "variantId" },
                { new StringContent(EditVariant.product ?? ""), "product" },
                { new StringContent(EditVariant.colorName ?? ""), "colorName" },
                { new StringContent(EditVariant.colorCode ?? ""), "colorCode" },
                { new StringContent(EditVariant.storage ?? ""), "storage" },
                { new StringContent(EditVariant.price.ToString()), "price" },
                { new StringContent(EditVariant.stock_quantity.ToString()), "stock_quantity" },
                { new StringContent(EditVariant.slug ?? ""), "slug" }
            };
            if (EditVariant.existingImages != null && EditVariant.existingImages.Count > 0)
            {
                foreach (var img in EditVariant.existingImages)
                {
                    form.Add(new StringContent(img), "existingImages");
                }
            }
            if (Request.Form.Files != null && Request.Form.Files.Count > 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    var streamContent = new StreamContent(file.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    form.Add(streamContent, "images", file.FileName);
                }
            }
            var response = await _httpClient.PutAsync($"api/v1/admin/products/variant/{EditVariant.variantId}", form);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"Cập nhật variant thất bại: {responseContent}";
                return Page();
            }
            TempData["SuccessMessage"] = "Cập nhật variant thành công!";
            if (!string.IsNullOrEmpty(EditVariant.product))
                return Redirect($"/Admin/Products/Details/{EditVariant.product}");
            return RedirectToPage("/Admin/Products/Index");
        }
    }
}