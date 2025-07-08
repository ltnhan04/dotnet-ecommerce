using Amazon.Runtime;
using api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace api.Pages.Admin.Categories
{
    public class Create : PageModel
    {
        private readonly ILogger<Create> _logger;

        private readonly HttpClient _httpClient;
        [BindProperty]
        public CreateCategoryDto Category { get; set; } = new();
        public Create(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsJsonAsync("api/v1/admin/categories", Category);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonDocument.Parse(json);
                var categoryId = result.RootElement.GetProperty("data").GetProperty("_id").GetString();
                return RedirectToPage("./Details", new { id = categoryId });
            }
            ModelState.AddModelError(string.Empty, "Create failed.");
            return Page();
        }
    }
}