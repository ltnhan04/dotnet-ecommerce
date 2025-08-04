using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;

namespace api.Pages.Admin.Dashboard
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        public TotalDto? Summary { get; set; }
        public ChartResponseDto? ChartData { get; set; } 
        public List<TopProductDtoRes> TopProducts { get; set; } = new();
        public List<TopLocationDto> TopLocations { get; set; } = new();

        [BindProperty(SupportsGet = true)] 
        public DateTime FromDate { get; set; }

        [BindProperty(SupportsGet = true)] // Allow bind from query string
        public DateTime ToDate { get; set; }

        public async Task OnGetAsync()
        {
            if (FromDate == default)
            {
                FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }
            if (ToDate == default)
            {
                ToDate = DateTime.Today;
            }

            var http = new HttpClient();
            http.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL") ?? "https://localhost:8000");

            var accessToken = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(accessToken))
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            Summary = await http.GetFromJsonAsync<TotalDto>($"/v1/revenue/total?fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}");

            ChartData = await http.GetFromJsonAsync<ChartResponseDto>($"/v1/revenue/chart?fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}");

            TopProducts = await http.GetFromJsonAsync<List<TopProductDtoRes>>($"/v1/revenue/top10?fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}");
            TopLocations = await http.GetFromJsonAsync<List<TopLocationDto>>($"/v1/revenue/top-sales-by-location?fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}");
        }

        // Update AJAX endpoint for chart to get fromDate and toDate
        public async Task<JsonResult> OnGetChartDataAsync(DateTime fromDate, DateTime toDate)
        {
            var http = new HttpClient();
            http.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL") ?? "https://localhost:8000");

            var accessToken = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(accessToken))
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var chartResponse = await http.GetFromJsonAsync<ChartResponseDto>($"/v1/revenue/chart?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");
            return new JsonResult(chartResponse); // Return alls response object
        }

        public class TotalDto
        {
            public decimal totalAmount { get; set; } 
            public int totalOrder { get; set; }
            public int totalCustomer { get; set; }
            public int totalPendingOrder { get; set; }
        }

        public class RevenueDataPointDto 
        {
            public string label { get; set; }
            public decimal totalRevenue { get; set; } 
        }

        public class ChartResponseDto
        {
            public List<RevenueDataPointDto> data { get; set; } = new();
            public string granularity { get; set; } = string.Empty;
        }

        public class TopProductDtoRes
        {
            public string productName { get; set; } = string.Empty;
            public string image { get; set; } = string.Empty;
            public int totalSold { get; set; }
            public decimal price { get; set; } 
            public string productId { get; set; } = string.Empty; 
            public string variantId { get; set; } = string.Empty;
        }

        public class TopLocationDto
        {
            public string city { get; set; } = string.Empty; 
            public int totalSold { get; set; } 
        }
    }
}