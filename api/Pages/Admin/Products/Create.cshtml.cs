using Amazon.Runtime;
using api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Collections.Generic;

namespace api.Pages.Admin.Products
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        [BindProperty]
        public CreateProductDto Product { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public CreateModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var catRes = await _httpClient.GetAsync("api/v1/admin/categories");
            if (catRes.IsSuccessStatusCode)
            {
                var catJson = await catRes.Content.ReadAsStringAsync();
                Categories = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDto>>(System.Text.Json.JsonDocument.Parse(catJson).RootElement.GetProperty("data").ToString()) ?? new();
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

            var response = await _httpClient.PostAsJsonAsync("api/v1/admin/products", Product);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonDocument.Parse(json);
                var productId = result.RootElement.GetProperty("data").GetProperty("_id").GetString();
                return RedirectToPage("./Details", new { id = productId });
            }
            ModelState.AddModelError(string.Empty, "Create failed.");
            return Page();
        }
    }
}