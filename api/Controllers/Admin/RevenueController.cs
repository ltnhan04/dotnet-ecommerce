using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Interfaces.Services;
using api.Services.Admin;
using api.Dtos;
using Microsoft.AspNetCore.Authorization;
using api.Enums;
using api.Common;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/revenue")]
    [Authorize(Roles = "admin")]
    public class RevenueController : ControllerBase
    {
        private readonly IRevenueService _revenueService; 
        private readonly IGranularityHelper _granularityHelper;

        public RevenueController(IRevenueService revenueService, IGranularityHelper granularityHelper)
        {
            _revenueService = revenueService;
            _granularityHelper = granularityHelper;
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotal(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await _revenueService.GetTotalDashboardData(fromDate, toDate);
            return Ok(result);
        }

        [HttpGet("chart")]
        public async Task<IActionResult> GetRevenueChart(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            // Add: Logic default cho fromDate và toDate(now)
            if (fromDate == default) // Default value for DateTime if not provided
            {
                fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }
            if (toDate == default)
            {
                toDate = DateTime.Today;
            }
            // Đảm bảo fromDate là start day and end day
            fromDate = fromDate.Date;
            toDate = toDate.Date.AddDays(1).AddTicks(-1);

            // Granularity
            var recommendedGranularity = _granularityHelper.GetRecommendedGranularity(fromDate, toDate);

            // Take data with granularity
            var data = await _revenueService.GetRevenueChartData(fromDate, toDate, recommendedGranularity);

            // Return data and type of granularity used to frontend display suitable label
            return Ok(new
            {
                data = data,
                granularity = recommendedGranularity.ToString().ToLower() // Type string
            });
        }

        [HttpGet("top10")]
        public async Task<IActionResult> GetTop10BestSellingProducts(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await _revenueService.GetTop10BestSellingProducts(fromDate, toDate);
            return Ok(result);
        }

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