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
            var totalCustomer = await _context.Users.CountAsync(u => u.role == "user");

            return new TotalDto
            {
                totalOrder = totalOrder,
                totalAmount = totalAmount,
                totalCustomer = totalCustomer,
                totalPendingOrder = totalPendingOrder
            };
        }

        public async Task<List<RevenueDto>> getRevenueChart(string type)
        {
            DateTime now = DateTime.Now;
            DateTime from;
            DateTime to = now;

            // Xác định khoảng thời gian dựa vào type
            switch (type)
            {
                case "day":
                    from = now.Date; // Từ đầu ngày hôm nay
                    break;

                case "week":
                    int daysSinceMonday = ((int)now.DayOfWeek + 6) % 7; // chuyển Sunday=0 thành Sunday=6
                    from = now.Date.AddDays(-daysSinceMonday); // Từ thứ 2 đầu tuần
                    break;

                case "month":
                    from = new DateTime(now.Year, 1, 1); // Từ đầu năm
                    break;

                default:
                    throw new ArgumentException("Invalid type");
            }

            // Đảm bảo to là cuối ngày hiện tại
            to = to.Date.AddDays(1).AddTicks(-1);

            var orders = await _context.Orders
                .Where(o => o.status == "delivered" && o.createdAt >= from && o.createdAt <= to)
                .ToListAsync();

            List<RevenueDto> result;

            switch (type)
            {
                case "day":
                    // Chia theo giờ
                    result = Enumerable.Range(0, 24)
                        .Select(hour => new RevenueDto
                        {
                            label = $"{hour}h",
                            totalRevenue = orders
                                .Where(o => o.createdAt.Hour == hour)
                                .Sum(o => o.totalAmount)
                        }).ToList();
                    break;

                case "week":
                    string[] days = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
                    result = Enumerable.Range(0, 7)
                        .Select(i => new RevenueDto
                        {
                            label = days[i],
                            totalRevenue = orders
                                .Where(o => (int)o.createdAt.DayOfWeek == (i + 1) % 7)
                                .Sum(o => o.totalAmount)
                        }).ToList();
                    break;

                case "month":
                    string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                    result = Enumerable.Range(1, 12)
                        .Select(m => new RevenueDto
                        {
                            label = months[m - 1],
                            totalRevenue = orders
                                .Where(o => o.createdAt.Month == m)
                                .Sum(o => o.totalAmount)
                        }).ToList();
                    break;

                default:
                    throw new ArgumentException("Invalid type");
            }

            return result;
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
                    totalSold = g.Sum(x => x.quantity),
                })
                .OrderByDescending(x => x.totalSold)
                .Take(7)
                .ToList();

            // Chuyển đổi từ TopProductDto sang TopProductDtoRes
            List<TopProductDtoRes> listVariants = variantSales.Select(x => new TopProductDtoRes
            {
                variantId = x.variantId.ToString(),
                totalSold = x.totalSold,
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
                    item.image = variant.images.Count >= 4 ? variant.images[3] : variant.images.FirstOrDefault() ?? string.Empty;
                    item.price = variant.price;

                    var product = products.FirstOrDefault(p => p._id.ToString() == variant.product.ToString());
                    item.productName = (product?.name + " " + variant.color.colorName + " " + variant.storage) ?? "Unknown";
                }
            }

            return listVariants;
        }
        public async Task<List<TopSalesByLocationDto>> GetTopSalesByLocation()
        {
            var deliveredOrders = await _context.Orders
                .Where(o => o.status == "delivered")
                .ToListAsync();

            if (!deliveredOrders.Any())
                return new List<TopSalesByLocationDto>();

            var provinceCount = deliveredOrders
                .Select(o =>
                {
                    var addressParts = o.shippingAddress.Split(',');
                    string province = addressParts.Length >= 2
                        ? addressParts[^2].Trim()
                        : "Unknown";

                    return province;
                })
                .GroupBy(p => p)
                .Select(g => new TopSalesByLocationDto
                {
                    city = g.Key,
                    totalSold = g.Count()
                })
                .OrderByDescending(x => x.totalSold)
                .Take(5)
                .ToList();

            return provinceCount;
        }

    }
}