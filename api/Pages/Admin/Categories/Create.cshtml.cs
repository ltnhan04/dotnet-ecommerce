using Amazon.Runtime;
using api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;


namespace api.Pages.Admin.Categories
{
    [Authorize(Roles = "admin")]
    public class Create : PageModel
    {

        private readonly HttpClient _httpClient;
        [BindProperty]
        public CreateCategoryDto Category { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();

        public Create(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var catRes = await _httpClient.GetAsync("v1/admin/categories");
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

            var response = await _httpClient.PostAsJsonAsync("v1/admin/categories", Category);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonDocument.Parse(json);
                var categoryId = result.RootElement.GetProperty("data").GetProperty("_id").GetString();
                return RedirectToPage("/Admin/Categories/Index", new { id = categoryId });
            }
            ModelState.AddModelError(string.Empty, "Create failed.");
            return Page();
        }
    }
}