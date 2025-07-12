using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Interfaces.Services;
using api.Services.Admin;
using api.Dtos;
using Microsoft.AspNetCore.Authorization;
using api.Enums; // Thêm namespace cho Granularity enum
using api.Common; // Thêm namespace cho GranularityHelper

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/revenue")]
    [Authorize(Roles = "admin")]
    public class RevenueController : ControllerBase
    {
        private readonly IRevenueService _revenueService; // Sử dụng interface
        private readonly IGranularityHelper _granularityHelper; // Thêm helper

        public RevenueController(IRevenueService revenueService, IGranularityHelper granularityHelper)
        {
            _revenueService = revenueService;
            _granularityHelper = granularityHelper;
        }

        // Cập nhật endpoint để nhận fromDate và toDate
        [HttpGet("total")]
        public async Task<IActionResult> GetTotal(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await _revenueService.GetTotalDashboardData(fromDate, toDate);
            return Ok(result);
        }

        // Cập nhật endpoint để nhận fromDate, toDate và trả về cả granularity gợi ý
        [HttpGet("chart")]
        public async Task<IActionResult> GetRevenueChart(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            // THÊM: Logic mặc định cho fromDate và toDate nếu không được cung cấp (tháng hiện tại)
            if (fromDate == default) // Default value for DateTime if not provided
            {
                fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }
            if (toDate == default)
            {
                toDate = DateTime.Today;
            }
            // Đảm bảo fromDate là đầu ngày và toDate là cuối ngày
            fromDate = fromDate.Date;
            toDate = toDate.Date.AddDays(1).AddTicks(-1);

            // Xác định granularity được khuyến nghị
            var recommendedGranularity = _granularityHelper.GetRecommendedGranularity(fromDate, toDate);

            // Lấy dữ liệu biểu đồ với granularity đã xác định
            var data = await _revenueService.GetRevenueChartData(fromDate, toDate, recommendedGranularity);

            // Trả về cả dữ liệu và loại granularity đã dùng để frontend có thể biết cách hiển thị label
            return Ok(new
            {
                data = data,
                granularity = recommendedGranularity.ToString().ToLower() // Gửi lại dạng string
            });
        }

        // Cập nhật endpoint để nhận fromDate và toDate
        [HttpGet("top10")]
        public async Task<IActionResult> GetTop10BestSellingProducts(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await _revenueService.GetTop10BestSellingProducts(fromDate, toDate);
            return Ok(result);
        }

        // Cập nhật endpoint để nhận fromDate và toDate
        [HttpGet("top-sales-by-location")]
        public async Task<IActionResult> GetTopSalesByLocation(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await _revenueService.GetTopSalesByLocation(fromDate, toDate);
            return Ok(result);
        }
    }
}