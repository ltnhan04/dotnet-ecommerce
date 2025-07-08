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
        public string Name { get; set; }
        public string OldName { get; set; }
        public string OldParentName { get; set; }
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
            // Gọi endpoint lấy tất cả danh mục cha
            var response = await _httpClient.GetAsync("api/v1/admin/categories");
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
                    Name = cat.name;
                    OldName = cat.name;
                    if (!string.IsNullOrEmpty(cat.parent_category))
                    {
                        // Lấy tên danh mục cha
                        var parentCat = list.FirstOrDefault(c => c._id == cat.parent_category);
                        if (parentCat != null)
                        {
                            OldParentName = parentCat.name;
                        }
                        else
                        {
                            // Nếu không tìm thấy trong list cha, thử lấy qua API sub
                            var subResponse = await _httpClient.GetAsync($"api/v1/admin/categories/sub/{cat.parent_category}");
                            if (subResponse.IsSuccessStatusCode)
                            {
                                var subJson = await subResponse.Content.ReadAsStringAsync();
                                var subDoc = System.Text.Json.JsonDocument.Parse(subJson);
                                var subData = subDoc.RootElement.GetProperty("data");
                                var subList = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDto>>(subData.GetRawText()) ?? new();
                                var subCat = subList.FirstOrDefault(c => c._id == cat.parent_category);
                                if (subCat != null)
                                {
                                    OldParentName = subCat.name;
                                }
                            }
                        }
                    }
                    return;
                }
            }
            // Nếu không tìm thấy ở danh mục cha, thử lấy theo parent (danh mục con)
            var subResponse2 = await _httpClient.GetAsync($"api/v1/admin/categories/sub/{Id}");
            if (subResponse2.IsSuccessStatusCode)
            {
                var json = await subResponse2.Content.ReadAsStringAsync();
                var doc = System.Text.Json.JsonDocument.Parse(json);
                var data = doc.RootElement.GetProperty("data");
                var subList = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDto>>(data.GetRawText()) ?? new();
                var subCat = subList.FirstOrDefault(c => c._id == Id);
                if (subCat != null)
                {
                    Category = new CreateCategoryDto { _id = subCat._id, name = subCat.name, parent_category = subCat.parent_category };
                    Name = subCat.name;
                    OldName = subCat.name;
                    if (!string.IsNullOrEmpty(subCat.parent_category))
                    {
                        // Lấy tên danh mục cha
                        var parentCat = subList.FirstOrDefault(c => c._id == subCat.parent_category);
                        if (parentCat != null)
                        {
                            OldParentName = parentCat.name;
                        }
                        else
                        {
                            var parentResponse = await _httpClient.GetAsync($"api/v1/admin/categories/{subCat.parent_category}");
                            if (parentResponse.IsSuccessStatusCode)
                            {
                                var parentJson = await parentResponse.Content.ReadAsStringAsync();
                                var parentDoc = System.Text.Json.JsonDocument.Parse(parentJson);
                                var parentData = parentDoc.RootElement.GetProperty("data");
                                var parentList = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDto>>(parentData.GetRawText()) ?? new();
                                var parent = parentList.FirstOrDefault(c => c._id == subCat.parent_category);
                                if (parent != null)
                                {
                                    OldParentName = parent.name;
                                }
                            }
                        }
                    }
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