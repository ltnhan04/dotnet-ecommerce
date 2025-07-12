using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;
using api.Dtos;
using api.Enums; // Thêm namespace cho Granularity enum
using api.Common; // Thêm namespace cho GranularityHelper


namespace api.Interfaces.Services
{
    public interface IRevenueService
    {
        // Cập nhật phương thức để nhận fromDate và toDate
        Task<TotalDto> GetTotalDashboardData(DateTime? fromDate, DateTime? toDate);

        // Cập nhật phương thức để nhận fromDate, toDate và granularity
        Task<List<RevenueDto>> GetRevenueChartData(DateTime fromDate, DateTime toDate, Granularity granularity);

        // Cập nhật phương thức để nhận fromDate và toDate
        Task<List<TopProductDtoRes>> GetTop10BestSellingProducts(DateTime? fromDate, DateTime? toDate);

        // Cập nhật phương thức để nhận fromDate và toDate
        Task<List<TopSalesByLocationDto>> GetTopSalesByLocation(DateTime? fromDate, DateTime? toDate);
    }
}