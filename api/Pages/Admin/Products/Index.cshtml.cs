using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using api.Dtos;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace api.Pages.Admin.Products
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<ProductDto> Products { get; set; } = [];
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; } = 0;
        public string? Search { get; set; }
        public string? CategoryId { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public string? Color { get; set; }
        public string? Storage { get; set; }
        public string? Status { get; set; }
        public int? Rating { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();

        public IndexModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public async Task OnGetAsync(string? search, string? categoryId,[FromQuery] int page = 1,[FromQuery] int size = 10, int? minPrice = null, int? maxPrice = null, string? color = null, string? storage = null)
        {   
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var catRes = await _httpClient.GetAsync("api/v1/admin/categories");
            if (catRes.IsSuccessStatusCode)
            {
                var catJson = await catRes.Content.ReadAsStringAsync();
                var catDoc = JsonDocument.Parse(catJson);
                Categories = JsonSerializer.Deserialize<List<CategoryDto>>(catDoc.RootElement.GetProperty("data").ToString())!;
            }
            Search = search;
            CategoryId = categoryId;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            Color = color;
            Storage = storage;
            var url = $"api/v1/admin/products/search?page={page}&size={size}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            if (!string.IsNullOrEmpty(categoryId)) url += $"&categoryId={Uri.EscapeDataString(categoryId)}";
            if (minPrice.HasValue) url += $"&minPrice={minPrice.Value}";
            if (maxPrice.HasValue) url += $"&maxPrice={maxPrice.Value}";
            if (!string.IsNullOrEmpty(color)) url += $"&color={Uri.EscapeDataString(color)}";
            if (!string.IsNullOrEmpty(storage)) url += $"&storage={Uri.EscapeDataString(storage)}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(json);
                var data = result.RootElement.GetProperty("data");
                Products = JsonSerializer.Deserialize<List<ProductDto>>(data.GetProperty("items").ToString())!;
                CurrentPage = data.GetProperty("page").GetInt32();
                PageSize = data.GetProperty("size").GetInt32();
                TotalCount = data.GetProperty("total").GetInt32();
                TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }
    }
}