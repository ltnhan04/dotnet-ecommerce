using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using api.Dtos;
using System.Net.Http.Json;

namespace api.Pages.Admin.Categories
{
    public class Update : PageModel
    {
        private readonly ILogger<Update> _logger;
        private readonly HttpClient _httpClient;
        [BindProperty]
        public CreateCategoryDto Category { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public Update(ILogger<Update> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public async Task OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id)) return;
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetAsync($"api/v1/admin/categories");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = System.Text.Json.JsonDocument.Parse(json);
                var data = doc.RootElement.GetProperty("data");
                var list = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDto>>(data.GetRawText()) ?? new();
                var cat = list.FirstOrDefault(c => c._id == Id);
                if (cat != null)
                {
                    Category = new CreateCategoryDto { _id = cat._id, name = cat.name, parent_category = cat.parent_category };
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.PutAsJsonAsync($"api/v1/admin/categories/{Id}", Category);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            ModelState.AddModelError(string.Empty, "Update failed.");
            return Page();
        }
    }
}