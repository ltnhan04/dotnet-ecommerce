using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;


namespace api.Interfaces.Repositories
{
    public interface IRevenueRepository
    {
        Task<TotalDto> GetTotalDashboardData(DateTime? fromDate = null, DateTime? toDate = null);

        Task<List<RevenueDto>> GetRevenueChartData(DateTime fromDate, DateTime toDate, string granularity);

        Task<List<TopProductDtoRes>> GetTop10BestSellingProducts(DateTime? fromDate = null, DateTime? toDate = null);

        Task<List<TopSalesByLocationDto>> GetTopSalesByLocation(DateTime? fromDate = null, DateTime? toDate = null);
    }
}