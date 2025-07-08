using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace api.Pages.Admin.Dashboard
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        public TotalDto? Summary { get; set; }
        public List<RevenueChartDto> ChartData { get; set; } = new();
        public List<TopProductDtoRes> TopProducts { get; set; } = new();
        public List<TopLocationDto> TopLocations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var http = new HttpClient();
            http.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL") ?? "https://localhost:8000");

            var accessToken = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(accessToken))
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            Summary = await http.GetFromJsonAsync<TotalDto>("/api/v1/revenue/total");
            ChartData = await http.GetFromJsonAsync<List<RevenueChartDto>>("/api/v1/revenue/chart?type=day"); // default là day
            TopProducts = await http.GetFromJsonAsync<List<TopProductDtoRes>>("/api/v1/revenue/top10");
            TopLocations = await http.GetFromJsonAsync<List<TopLocationDto>>("/api/v1/revenue/top-sales-by-location");
        }

        // AJAX endpoint cho biểu đồ theo type (day/week/month)
        public async Task<JsonResult> OnGetChartDataAsync(string type)
        {
            var http = new HttpClient();
            http.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL") ?? "https://localhost:8000");

            var accessToken = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(accessToken))
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var chartData = await http.GetFromJsonAsync<List<RevenueChartDto>>($"/api/v1/revenue/chart?type={type}");
            return new JsonResult(chartData);
        }

        public class TotalDto
        {
            public int totalAmount { get; set; }
            public int totalOrder { get; set; }
            public int totalCustomer { get; set; }
            public int totalPendingOrder { get; set; }
        }

        public class RevenueChartDto
        {
            public string label { get; set; }
            public int totalRevenue { get; set; }
        }

        public class TopProductDtoRes
        {
            public string productName { get; set; }
            public string image { get; set; }
            public int totalSold { get; set; }
        }

        public class TopLocationDto
        {
            public string province { get; set; }
            public int totalOrder { get; set; }
        }
    }
}
