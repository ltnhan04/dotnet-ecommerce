using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Interfaces.Services;
using api.Interfaces.Repositories;
using api.Dtos;
using api.Enums; // Thêm namespace cho Granularity enum
using api.Common; // Thêm namespace cho GranularityHelper

namespace api.Services.Admin
{
    public class RevenueService : IRevenueService // Kế thừa từ interface
    {
        private readonly IRevenueRepository _revenueRepository;
        private readonly IGranularityHelper _granularityHelper; // Thêm helper

        public RevenueService(IRevenueRepository revenueRepository, IGranularityHelper granularityHelper)
        {
            _revenueRepository = revenueRepository;
            _granularityHelper = granularityHelper;
        }

        // Cập nhật phương thức để nhận fromDate và toDate
        public async Task<TotalDto> GetTotalDashboardData(DateTime? fromDate, DateTime? toDate)
        {
            return await _revenueRepository.GetTotalDashboardData(fromDate, toDate);
        }

        // Cập nhật phương thức để nhận fromDate, toDate và granularity
        public async Task<List<RevenueDto>> GetRevenueChartData(DateTime fromDate, DateTime toDate, Granularity granularity)
        {
            // Không cần tính granularity ở đây vì Controller sẽ gửi nó đến.
            // Service chỉ chuyển tiếp yêu cầu đến Repository.
            return await _revenueRepository.GetRevenueChartData(fromDate, toDate, granularity.ToString().ToLower());
        }

        // Cập nhật phương thức để nhận fromDate và toDate
        public async Task<List<TopProductDtoRes>> GetTop10BestSellingProducts(DateTime? fromDate, DateTime? toDate)
        {
            return await _revenueRepository.GetTop10BestSellingProducts(fromDate, toDate);
        }

        // Cập nhật phương thức để nhận fromDate và toDate
        public async Task<List<TopSalesByLocationDto>> GetTopSalesByLocation(DateTime? fromDate, DateTime? toDate)
        {
            return await _revenueRepository.GetTopSalesByLocation(fromDate, toDate);
        }
    }
}