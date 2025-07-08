using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api.Pages.Admin.Orders
{
    [Authorize(Roles = "admin")]
    public class DetailModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        [BindProperty]
        public string SelectedStatus { get; set; }

        [BindProperty]
        public bool ConfirmUpdate { get; set; }
        public List<string> ValidStatus { get; set; } = new();
        public AdminGetAllOrder DetailOrder { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var url = $"api/v1/admin/orders/{id}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(json);
                var data = result.RootElement.GetProperty("data");
                DetailOrder = JsonSerializer.Deserialize<AdminGetAllOrder>(data.GetRawText()) ?? new AdminGetAllOrder();
            }

            var validTransitions = new Dictionary<string, List<string>>
            {
                ["pending"] = new List<string> { "processing", "cancel" },
                ["processing"] = new List<string> { "shipped", "cancel" },
                ["shipped"] = new List<string> { "delivered", "cancel" }
            };

            var currentStatus = DetailOrder.status?.ToLower() ?? "pending";
            ValidStatus = validTransitions.ContainsKey(currentStatus)
                ? validTransitions[currentStatus]
                : new List<string>();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string id)
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var updateUrl = $"api/v1/admin/orders/{id}";
            var payload = new { status = SelectedStatus };
            var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var updateResponse = await _httpClient.PutAsync(updateUrl, content);

            if (!updateResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to update order status.";
                return RedirectToPage("./Index");
            }

            TempData["SuccessMessage"] = "Order status updated successfully.";
            return RedirectToPage("./Index");
        }
    }
}