using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;
using api.Dtos;
using api.Enums; 
using api.Common; 


namespace api.Interfaces.Services
{
    public interface IRevenueService
    {
        Task<TotalDto> GetTotalDashboardData(DateTime? fromDate, DateTime? toDate);

        Task<List<RevenueDto>> GetRevenueChartData(DateTime fromDate, DateTime toDate, Granularity granularity);

        Task<List<TopProductDtoRes>> GetTop10BestSellingProducts(DateTime? fromDate, DateTime? toDate);

        Task<List<TopSalesByLocationDto>> GetTopSalesByLocation(DateTime? fromDate, DateTime? toDate);
    }
}