using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using api.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace api.Pages.Admin.Products
{
    [Authorize(Roles = "admin")]
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ProductDto Product { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; } = string.Empty;
        public DeleteModel(IHttpClientFactory factory)
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
             var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.DeleteAsync($"api/v1/admin/products/{Id}");
            return response.IsSuccessStatusCode ? RedirectToPage("./Index") : Page();
        }
    }
}