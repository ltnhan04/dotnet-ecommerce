using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;
using api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;


namespace api.Repositories.Admin
{
    public class RevenueRepository : IRevenueRepository
    {
        private readonly iTribeDbContext _context;
        private readonly ILogger<RevenueRepository> _logger;
        public RevenueRepository(iTribeDbContext context, ILogger<RevenueRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<TotalDto> getTotalRevenue()
        {
            var orders = await _context.Orders.ToListAsync();

            var totalOrder = orders.Count;
            var totalAmount = orders
                .Where(o => o.status == "delivered")
                .Sum(o => o.totalAmount);
            var totalPendingOrder = orders.Count(o => o.status == "pending");
            var totalCustomer = await _context.Users.CountAsync(u => u.role =="user");

            return new TotalDto
            {
                totalOrder = totalOrder,
                totalAmount = totalAmount,
                totalCustomer = totalCustomer,
                totalPendingOrder = totalPendingOrder
            };
        }

        public async Task<List<RevenueDto>> getRevenueChart(string type, DateTime from, DateTime to)
        {
            // Đảm bảo to luôn là cuối ngày
            to = to.Date.AddDays(1).AddTicks(-1);
            var orders = await _context.Orders
                .Where(o => o.status == "delivered" && o.createdAt >= from && o.createdAt <= to)
                .ToListAsync();

            List<RevenueDto> result;

            if (from.Date == to.Date.Date)
            {
                // Trường hợp thống kê trong 1 ngày → theo giờ
                result = orders
                    .GroupBy(o => o.createdAt.Hour)
                    .Select(g => new RevenueDto
                    {
                        label = $"{g.Key:00}:00",
                        totalRevenue = g.Sum(o => o.totalAmount)
                    })
                    .OrderBy(r => r.label)
                    .ToList();

                return result;
            }

            result = type switch
            {
                "day" => orders
                    .GroupBy(o => o.createdAt.Date)
                    .Select(g => new RevenueDto
                    {
                        label = g.Key.ToString("yyyy-MM-dd"),
                        totalRevenue = g.Sum(o => o.totalAmount)
                    }).ToList(),

                "month" => orders
                    .GroupBy(o => new { o.createdAt.Year, o.createdAt.Month })
                    .Select(g => new RevenueDto
                    {
                        label = $"{g.Key.Year}-{g.Key.Month:D2}",
                        totalRevenue = g.Sum(o => o.totalAmount)
                    }).ToList(),

                "year" => orders
                    .GroupBy(o => o.createdAt.Year)
                    .Select(g => new RevenueDto
                    {
                        label = g.Key.ToString(),
                        totalRevenue = g.Sum(o => o.totalAmount)
                    }).ToList(),

                _ => throw new ArgumentException("Invalid type")
            };

            return result.OrderBy(r => r.label).ToList();
        }

        public async Task<List<TopProductDtoRes>> GetTop10BestSellingProducts()
        {
            // Bước 1: Lấy tất cả đơn hàng có trạng thái 'delivered'
            var deliveredOrders = await _context.Orders
                .Where(o => o.status == "delivered")
                .ToListAsync();

            if (!deliveredOrders.Any())
                return new List<TopProductDtoRes>();

            // Bước 2: Lấy tất cả các variants từ đơn hàng
            var allOrderVariants = deliveredOrders
                .SelectMany(o => o.variants)
                .ToList();

            if (!allOrderVariants.Any())
                return new List<TopProductDtoRes>();

            // Bước 3: Đếm số lượng bán theo variantId
            var variantSales = allOrderVariants
                .GroupBy(v => v.variant)
                .Select(g => new TopProductDto
                {
                    variantId = g.Key,
                    totalSold = g.Sum(x => x.quantity)
                })
                .OrderByDescending(x => x.totalSold)
                .Take(10)
                .ToList();

            // Chuyển đổi từ TopProductDto sang TopProductDtoRes
            List<TopProductDtoRes> listVariants = variantSales.Select(x => new TopProductDtoRes
            {
                variantId = x.variantId.ToString(),
                totalSold = x.totalSold
            }).ToList();

            var variantIds = listVariants.Select(r => ObjectId.Parse(r.variantId)).ToList();

            var productVariants = await _context.ProductVariants
                .Where(v => variantIds.Contains(v._id))
                .ToListAsync();

            _logger.LogInformation("Processing!");

            var productIds = productVariants.Select(v => v.product).Distinct().ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p._id))
                .ToListAsync();

            foreach (var item in listVariants)
            {
                var variant = productVariants.FirstOrDefault(v => v._id == ObjectId.Parse(item.variantId));
                if (variant != null)
                {
                    item.productId = variant.product.ToString();
                    item.image = variant.images.FirstOrDefault() ?? string.Empty;
                    item.price = variant.price;

                    var product = products.FirstOrDefault(p => p._id.ToString() == variant.product.ToString());
                    item.productName = (product?.name+" "+variant.color.colorName+" "+variant.storage) ?? "Unknown";
                }
            }

            return listVariants;
        }

    }
}