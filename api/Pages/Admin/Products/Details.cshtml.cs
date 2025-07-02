using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using api.Dtos;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace api.Pages.Admin.Products
{
    [Authorize(Roles = "admin")]
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DetailsModel> _logger;
        public ProductDto Product { get; set; } = new();
        public DetailsModel(IHttpClientFactory factory, ILogger<DetailsModel> logger)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
            _logger = logger;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetAsync($"api/v1/admin/products/{id}");
            if (!response.IsSuccessStatusCode) return RedirectToPage("./Index");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(json);
            Product = JsonSerializer.Deserialize<ProductDto>(result.RootElement.GetProperty("data").ToString())!;
            return Page();
        }
    }
}