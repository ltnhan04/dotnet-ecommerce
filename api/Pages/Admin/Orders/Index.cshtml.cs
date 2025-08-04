using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using api.Dtos;
using api.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api.Pages.Admin.Orders
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public IndexModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }
        public List<AdminGetAllOrder> Orders { get; set; } = new();

        [BindProperty(SupportsGet = true, Name = "page")]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true, Name = "size")]
        public int SizeOrder { get; set; } = 10;
        public int TotalCount { get; set; } = 0;
        public int TotalPages { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string OrderId { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string Customer { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string Email { get; set; } = string.Empty;
        public string Payment { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string Status { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string PaymentStatus { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public DateTime? DateFrom { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? DateTo { get; set; }

        public async Task OnGetAsync([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            CurrentPage = page;
            SizeOrder = size;
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var url = $"v1/admin/orders?page={CurrentPage}&size={SizeOrder}";

            if (!string.IsNullOrEmpty(OrderId)) url += $"&orderId={OrderId}";
            if (!string.IsNullOrEmpty(Customer)) url += $"&customer={Customer}";
            if (!string.IsNullOrEmpty(Status)) url += $"&status={Status}";
            if (!string.IsNullOrEmpty(PaymentStatus)) url += $"&paymentStatus={PaymentStatus}";
            if (!string.IsNullOrEmpty(Email)) url += $"&email={Email}";
            if (!string.IsNullOrEmpty(DateFrom.ToString())) url += $"&DateFrom={DateFrom}";
            if (!string.IsNullOrEmpty(DateTo.ToString())) url += $"&DateTo={DateTo}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(json);
                var data = result.RootElement.GetProperty("data");
                Orders = JsonSerializer.Deserialize<List<AdminGetAllOrder>>(data.GetProperty("items").ToString())!;
                TotalCount = data.GetProperty("total").GetInt32();
                TotalPages = (int)Math.Ceiling((double)TotalCount / SizeOrder);
            }
        }
    }
}