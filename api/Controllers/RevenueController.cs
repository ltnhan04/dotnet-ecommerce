using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Services.Admin;
using api.Dtos;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/revenue")]
    // [Authorize]
    public class RevenueController : ControllerBase
    {
        private readonly RevenueService _revenueService;
        public RevenueController(RevenueService revenueService)
        {
            _revenueService = revenueService;
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotal()
        {
            var result = await _revenueService.getTotalRevenue();
            return Ok(result);
        }

        [HttpGet("chart")]
        public async Task<IActionResult> GetRevenueChart([FromQuery] string type, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var data = await _revenueService.getRevenueChart(type, from, to);
            return Ok(data);
        }
        [HttpGet("top10")]
        public async Task<IActionResult> GetTop10BestSellingProducts()
        {
            var result = await _revenueService.GetTop10BestSellingProducts();
            return Ok(result);
        }
        [HttpGet("top-sales-by-location")]
        public async Task<IActionResult> GetTopSalesByLocation()
        {
            var result = await _revenueService.GetTopSalesByLocation();
            return Ok(result);
        }
    }
}