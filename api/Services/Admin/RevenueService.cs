using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Interfaces.Repositories;
using api.Dtos;


namespace api.Services.Admin
{
    public class RevenueService
    {
        private readonly IRevenueRespository revenueRespository;

        public RevenueService(IRevenueRespository revenueRespository)
        {
            this.revenueRespository = revenueRespository;
        }

        public async Task<TotalDto> getTotalRevenue()
        {
            return await revenueRespository.getTotalRevenue();
        }
        public async Task<List<RevenueDto>> getRevenueChart(string type, DateTime from, DateTime to)
        {
            return await revenueRespository.getRevenueChart(type, from, to);
        }
        public async Task<List<TopProductDtoRes>> GetTop10BestSellingProducts()
        {
            return await revenueRespository.GetTop10BestSellingProducts();
        }
    }
}