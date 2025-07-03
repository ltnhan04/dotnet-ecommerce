using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace api.Pages.Admin.Orders
{
    public class UpdateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UpdateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var body = new
            {
                status = Status.ToLower()
            };
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"api/v1/admin/orders/{OrderId}", content);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Updated status order failed");
                return Page();
            }
            TempData["Success"] = "Updated status order successfully";
            return RedirectToPage("Index");
        }
    }
}