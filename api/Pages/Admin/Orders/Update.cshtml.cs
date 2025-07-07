using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

         public UpdateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }
        
        [BindProperty]
        public string OrderId { get; set; } = string.Empty;
        
        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public string Status { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToPage("./Index");
            }

            OrderId = id;
             
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var response = await _httpClient.GetAsync($"api/v1/admin/orders/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonDocument.Parse(json);
                    var data = result.RootElement.GetProperty("data");
                    
                    if (data.TryGetProperty("status", out var statusElement))
                    {
                        Status = statusElement.GetString() ?? "pending";
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Không thể tải thông tin đơn hàng");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi tải thông tin đơn hàng: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(OrderId))
            {
                OrderId = RouteData.Values["id"]?.ToString() ?? "";
            }

            if (string.IsNullOrEmpty(OrderId))
            {
                ModelState.AddModelError("", "Order ID không hợp lệ");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                await LoadOrderData();
                return Page();
            }

            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var body = new
                {
                    status = Status.ToLower()
                };
                var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/v1/admin/orders/{OrderId}", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Cập nhật thất bại: {response.StatusCode} - {errorContent}");
                    await LoadOrderData();
                    return Page();
                }
                
                TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi kết nối: {ex.Message}");
                await LoadOrderData();
                return Page();
            }
        }

        private async Task LoadOrderData()
        {
            try
            {
                var token = Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync($"api/v1/admin/orders/{OrderId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonDocument.Parse(json);
                    var data = result.RootElement.GetProperty("data");
                    
                    if (data.TryGetProperty("status", out var statusElement))
                    {
                        Status = statusElement.GetString() ?? "pending";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order data: {ex.Message}");
            }
        }
    }
}