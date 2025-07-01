using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using api.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http.Json;
using System.Collections.Generic;

namespace api.Pages.Admin.Products
{
    [Authorize(Roles = "admin")]
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public UpdateProductDto UpdateProduct { get; set; } = new();
        public ProductDto Product { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; } = string.Empty;

        public EditModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public async Task<IActionResult> OnGetAsync()
        {
             var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetAsync($"api/v1/admin/products/{Id}");
            if (!response.IsSuccessStatusCode) return RedirectToPage("./Index");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(json);
            Product = JsonSerializer.Deserialize<ProductDto>(result.RootElement.GetProperty("data").ToString())!;
            UpdateProduct = new UpdateProductDto
            {
                name = Product.name,
                description = Product.description,
                category = Product.category._id
            };
            var catRes = await _httpClient.GetAsync("api/v1/admin/categories");
            if (catRes.IsSuccessStatusCode)
            {
                var catJson = await catRes.Content.ReadAsStringAsync();
                Categories = JsonSerializer.Deserialize<List<CategoryDto>>(JsonDocument.Parse(catJson).RootElement.GetProperty("data").ToString()) ?? new();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
             var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PutAsJsonAsync($"api/v1/admin/products/{Id}", UpdateProduct);
            return response.IsSuccessStatusCode ? RedirectToPage("./Index") : Page();
        }
    }
}