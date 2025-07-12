using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using api.Dtos;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;


namespace api.Pages.Admin.Categories
{
    [Authorize(Roles = "admin")]

    public class Index : PageModel
    {
        private readonly ILogger<Index> _logger;
        private readonly HttpClient _httpClient;

        public List<CategoryDto> Parents { get; set; } = new();
        public List<CategoryDto> Children { get; set; } = new();

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
            // Lấy danh mục cha
            var parentResponse = await _httpClient.GetAsync("api/v1/admin/categories");
            if (parentResponse.IsSuccessStatusCode)
            {
                var json = await parentResponse.Content.ReadAsStringAsync();
                var doc = System.Text.Json.JsonDocument.Parse(json);
                var data = doc.RootElement.GetProperty("data");
                Parents = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDto>>(data.GetRawText()) ?? new();
                Children = new List<CategoryDto>();
                // Lấy children cho từng parent
                foreach (var parent in Parents)
                {
                    var subResponse = await _httpClient.GetAsync($"api/v1/admin/categories/sub/{parent._id}");
                    if (subResponse.IsSuccessStatusCode)
                    {
                        var subJson = await subResponse.Content.ReadAsStringAsync();
                        var subDoc = System.Text.Json.JsonDocument.Parse(subJson);
                        var subData = subDoc.RootElement.GetProperty("data");
                        var children = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDto>>(subData.GetRawText()) ?? new();
                        Children.AddRange(children);
                    }
                }
            }
        }
    }
}
