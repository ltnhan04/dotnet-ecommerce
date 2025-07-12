using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System; // Thêm để sử dụng DateTime

namespace api.Pages.Admin.Dashboard
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        public TotalDto? Summary { get; set; }
        public ChartResponseDto? ChartData { get; set; } // Thay đổi để nhận object có cả data và granularity
        public List<TopProductDtoRes> TopProducts { get; set; } = new();
        public List<TopLocationDto> TopLocations { get; set; } = new();

        // Thêm các thuộc tính để nhận ngày từ người dùng
        [BindProperty(SupportsGet = true)] // Cho phép bind từ query string
        public DateTime FromDate { get; set; }

        [BindProperty(SupportsGet = true)] // Cho phép bind từ query string
        public DateTime ToDate { get; set; }

        public async Task OnGetAsync()
        {
            // Thiết lập giá trị mặc định cho FromDate và ToDate nếu không được cung cấp
            // Mặc định là tháng hiện tại
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

            // Gửi FromDate và ToDate cho tất cả các API call
            Summary = await http.GetFromJsonAsync<TotalDto>($"/api/v1/revenue/total?fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}");

            // ChartData giờ đây nhận từ /api/v1/revenue/chart
            ChartData = await http.GetFromJsonAsync<ChartResponseDto>($"/api/v1/revenue/chart?fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}");

            TopProducts = await http.GetFromJsonAsync<List<TopProductDtoRes>>($"/api/v1/revenue/top10?fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}");
            TopLocations = await http.GetFromJsonAsync<List<TopLocationDto>>($"/api/v1/revenue/top-sales-by-location?fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}");
        }

        // Cập nhật AJAX endpoint cho biểu đồ để nhận fromDate và toDate
        public async Task<JsonResult> OnGetChartDataAsync(DateTime fromDate, DateTime toDate)
        {
            var http = new HttpClient();
            http.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL") ?? "https://localhost:8000");

            var accessToken = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(accessToken))
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // Gọi API chart với fromDate và toDate
            var chartResponse = await http.GetFromJsonAsync<ChartResponseDto>($"/api/v1/revenue/chart?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");
            return new JsonResult(chartResponse); // Trả về toàn bộ response object
        }

        // --- Các DTOs cần được điều chỉnh hoặc thêm mới để khớp với Backend ---
        public class TotalDto
        {
            public decimal totalAmount { get; set; } // Sử dụng decimal cho tiền tệ
            public int totalOrder { get; set; }
            public int totalCustomer { get; set; }
            public int totalPendingOrder { get; set; }
        }

        public class RevenueDataPointDto // DTO cho từng điểm dữ liệu trong biểu đồ
        {
            public string label { get; set; }
            public decimal totalRevenue { get; set; } // Sử dụng decimal
        }

        public class ChartResponseDto // DTO mới để nhận cả dữ liệu và granularity
        {
            public List<RevenueDataPointDto> data { get; set; } = new();
            public string granularity { get; set; } = string.Empty;
        }

        public class TopProductDtoRes
        {
            public string productName { get; set; } = string.Empty;
            public string image { get; set; } = string.Empty;
            public int totalSold { get; set; }
            public decimal price { get; set; } // Thêm thuộc tính price nếu bạn muốn hiển thị
            public string productId { get; set; } = string.Empty; // Thêm productId và variantId
            public string variantId { get; set; } = string.Empty;
        }

        public class TopLocationDto
        {
            public string city { get; set; } = string.Empty; // Đổi từ province thành city cho khớp backend
            public int totalSold { get; set; } // Đổi từ totalOrder thành totalSold cho khớp backend
        }
    }
}