using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using api.Dtos;
using System.Collections.Generic;
using System.Text.Json;

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

        public IndexModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public async Task OnGetAsync(int page = 1, int size = 20)
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetAsync($"api/v1/admin/products?size={size}&page={page}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(json);
                Products = JsonSerializer.Deserialize<List<ProductDto>>(result.RootElement.GetProperty("data").ToString())!;
                CurrentPage = page;
                PageSize = size;
                if (result.RootElement.TryGetProperty("total", out var totalProp))
                {
                    TotalCount = totalProp.GetInt32();
                }
                else
                {
                    TotalCount = Products.Count;
                }
                TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }
    }
}