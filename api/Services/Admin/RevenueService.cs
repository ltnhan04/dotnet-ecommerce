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
        private readonly IRevenueRepository revenueRepository;

        public RevenueService(IRevenueRepository revenueRepository)
        {
            this.revenueRepository = revenueRepository;
        }

        public async Task<TotalDto> getTotalRevenue()
        {
            return await revenueRepository.getTotalRevenue();
        }
        public async Task<List<RevenueDto>> getRevenueChart(string type)
        {
            return await revenueRepository.getRevenueChart(type);
        }
        public async Task<List<TopProductDtoRes>> GetTop10BestSellingProducts()
        {
            return await revenueRepository.GetTop10BestSellingProducts();
        }
        public async Task<List<TopSalesByLocationDto>> GetTopSalesByLocation()
        {
            return await revenueRepository.GetTopSalesByLocation();
        }
    }
}