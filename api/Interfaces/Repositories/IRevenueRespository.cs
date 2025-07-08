using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;


namespace api.Interfaces.Repositories
{
    public interface IRevenueRepository
    {
        Task<TotalDto> getTotalRevenue();
        Task<List<RevenueDto>> getRevenueChart(string type);
        Task<List<TopProductDtoRes>> GetTop10BestSellingProducts();
        Task<List<TopSalesByLocationDto>> GetTopSalesByLocation();
    }
}