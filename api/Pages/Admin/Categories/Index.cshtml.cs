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
    public class Index : PageModel
    {
        private readonly ILogger<Index> _logger;
        private readonly HttpClient _httpClient;
        public List<CategoryDto> Categories { get; set; } = new();

        public Index(ILogger<Index> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public async Task OnGetAsync()
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetAsync("api/v1/admin/categories");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = System.Text.Json.JsonDocument.Parse(json);
                var data = doc.RootElement.GetProperty("data");
                Categories = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDto>>(data.GetRawText()) ?? new();
            }
        }
    }
}