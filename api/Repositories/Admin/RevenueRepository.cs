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

        // --- Cập nhật phương thức lấy tổng quan (TotalDto) để hỗ trợ From-To ---
        public async Task<TotalDto> GetTotalDashboardData(DateTime? fromDate = null, DateTime? toDate = null)
        {
            // Lọc các đơn hàng đã được giao trong khoảng thời gian đã chọn (nếu có)
            var query = _context.Orders.AsQueryable();

            if (fromDate.HasValue)
            {
                // Đảm bảo từ đầu ngày
                query = query.Where(o => o.createdAt >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                // Đảm bảo đến cuối ngày (23:59:59.999)
                query = query.Where(o => o.createdAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            var orders = await query.ToListAsync();

            var totalOrder = orders.Count();
            var totalAmount = orders
                .Where(o => o.status == "delivered")
                .Sum(o => o.totalAmount);
            var totalPendingOrder = orders.Count(o => o.status == "pending");

            // totalCustomer thường không liên quan đến khoảng thời gian, nhưng nếu bạn muốn lọc user mới tạo trong khoảng đó, bạn cần thêm logic vào đây.
            // Hiện tại giữ nguyên là tổng số user role "user"
            var totalCustomer = await _context.Users.CountAsync(u => u.role == "user");

            return new TotalDto
            {
                totalOrder = totalOrder,
                totalAmount = totalAmount,
                totalCustomer = totalCustomer,
                totalPendingOrder = totalPendingOrder
            };
        }

        // --- Cập nhật phương thức lấy dữ liệu biểu đồ doanh thu ---
        // Bây giờ sẽ nhận fromDate, toDate và một string 'granularity' (hourly, daily, weekly, monthly, yearly)
        public async Task<List<RevenueDto>> GetRevenueChartData(DateTime fromDate, DateTime toDate, string granularity)
        {
            // Đảm bảo fromDate là đầu ngày và toDate là cuối ngày cho việc lọc ban đầu
            fromDate = fromDate.Date;
            toDate = toDate.Date.AddDays(1).AddTicks(-1);

            // Lấy tất cả các đơn hàng đã được giao trong khoảng thời gian đã chọn
            var orders = await _context.Orders
                .Where(o => o.status == "delivered" && o.createdAt >= fromDate && o.createdAt <= toDate)
                .ToListAsync();

            List<RevenueDto> result = new List<RevenueDto>();

            switch (granularity.ToLower())
            {
                case "hourly":
                    // Từ fromDate đến toDate, tính theo giờ
                    for (DateTime date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
                    {
                        for (int hour = 0; hour < 24; hour++)
                        {
                            var currentPointTime = date.AddHours(hour);
                            var nextPointTime = currentPointTime.AddHours(1);

                            var totalRevenue = orders
                                .Where(o => o.createdAt >= currentPointTime && o.createdAt < nextPointTime)
                                .Sum(o => o.totalAmount);

                            result.Add(new RevenueDto
                            {
                                label = currentPointTime.ToString("HH:mm"), // Ví dụ: "09:00"
                                totalRevenue = totalRevenue
                            });
                        }
                    }
                    break;

                case "daily":
                    // Từ fromDate đến toDate, tính theo ngày
                    for (DateTime date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
                    {
                        var nextDay = date.AddDays(1);
                        var totalRevenue = orders
                            .Where(o => o.createdAt >= date && o.createdAt < nextDay)
                            .Sum(o => o.totalAmount);

                        result.Add(new RevenueDto
                        {
                            label = date.ToString("dd/MM"), // Ví dụ: "01/07"
                            totalRevenue = totalRevenue
                        });
                    }
                    break;

                case "weekly":
                    // Tìm ngày đầu tiên của tuần đầu tiên trong fromDate
                    // Giả sử tuần bắt đầu từ Thứ Hai (Monday)
                    DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
                    DateTime currentWeekStart = fromDate.Date;
                    while (currentWeekStart.DayOfWeek != firstDayOfWeek)
                    {
                        currentWeekStart = currentWeekStart.AddDays(-1);
                    }

                    while (currentWeekStart <= toDate)
                    {
                        DateTime nextWeekStart = currentWeekStart.AddDays(7);
                        var totalRevenue = orders
                            .Where(o => o.createdAt >= currentWeekStart && o.createdAt < nextWeekStart)
                            .Sum(o => o.totalAmount);

                        result.Add(new RevenueDto
                        {
                            label = $"{currentWeekStart:dd/MM} - {nextWeekStart.AddDays(-1):dd/MM}", // Ví dụ: "01/07 - 07/07"
                            totalRevenue = totalRevenue
                        });
                        currentWeekStart = nextWeekStart;
                    }
                    break;

                case "monthly":
                    // Từ tháng của fromDate đến tháng của toDate, tính theo tháng
                    DateTime currentMonth = new DateTime(fromDate.Year, fromDate.Month, 1);
                    DateTime endMonth = new DateTime(toDate.Year, toDate.Month, 1);

                    while (currentMonth <= endMonth)
                    {
                        DateTime nextMonth = currentMonth.AddMonths(1);
                        var totalRevenue = orders
                            .Where(o => o.createdAt >= currentMonth && o.createdAt < nextMonth)
                            .Sum(o => o.totalAmount);

                        result.Add(new RevenueDto
                        {
                            label = currentMonth.ToString("MM/yyyy"), // Ví dụ: "07/2025"
                            totalRevenue = totalRevenue
                        });
                        currentMonth = nextMonth;
                    }
                    break;

                case "yearly":
                    // Từ năm của fromDate đến năm của toDate, tính theo năm
                    int currentYear = fromDate.Year;
                    int endYear = toDate.Year;

                    for (int year = currentYear; year <= endYear; year++)
                    {
                        DateTime yearStart = new DateTime(year, 1, 1);
                        DateTime nextYearStart = new DateTime(year + 1, 1, 1);
                        var totalRevenue = orders
                            .Where(o => o.createdAt >= yearStart && o.createdAt < nextYearStart)
                            .Sum(o => o.totalAmount);

                        result.Add(new RevenueDto
                        {
                            label = year.ToString(), // Ví dụ: "2025"
                            totalRevenue = totalRevenue
                        });
                    }
                    break;

                default:
                    _logger.LogWarning("Invalid granularity type provided: {Granularity}", granularity);
                    throw new ArgumentException("Invalid granularity type. Supported types: hourly, daily, weekly, monthly, yearly.");
            }

            return result;
        }

        // --- Cập nhật phương thức GetTop10BestSellingProducts để hỗ trợ From-To ---
        public async Task<List<TopProductDtoRes>> GetTop10BestSellingProducts(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Orders.AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.createdAt >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                query = query.Where(o => o.createdAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            var deliveredOrders = await query
                .Where(o => o.status == "delivered")
                .ToListAsync();

            if (!deliveredOrders.Any())
                return new List<TopProductDtoRes>();

            var allOrderVariants = deliveredOrders
                .SelectMany(o => o.variants)
                .ToList();

            if (!allOrderVariants.Any())
                return new List<TopProductDtoRes>();

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

            List<TopProductDtoRes> listVariants = variantSales.Select(x => new TopProductDtoRes
            {
                variantId = x.variantId.ToString(),
                totalSold = x.totalSold,
            }).ToList();

            var variantIds = listVariants.Select(r => ObjectId.Parse(r.variantId)).ToList();

            var productVariants = await _context.ProductVariants
                .Where(v => variantIds.Contains(v._id))
                .ToListAsync();

            _logger.LogInformation("Processing GetTop10BestSellingProducts!");

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
                    // Đảm bảo images có đủ phần tử trước khi truy cập index 3
                    item.image = variant.images.Count >= 4 ? variant.images[3] : variant.images.FirstOrDefault() ?? string.Empty;
                    item.price = variant.price;

                    var product = products.FirstOrDefault(p => p._id.ToString() == variant.product.ToString());
                    item.productName = (product?.name + " " + variant.color.colorName + " " + variant.storage) ?? "Unknown";
                }
            }

            return listVariants;
        }

        // --- Cập nhật phương thức GetTopSalesByLocation để hỗ trợ From-To ---
        public async Task<List<TopSalesByLocationDto>> GetTopSalesByLocation(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Orders.AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.createdAt >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                query = query.Where(o => o.createdAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            var deliveredOrders = await query
                .Where(o => o.status == "delivered")
                .ToListAsync();

            if (!deliveredOrders.Any())
                return new List<TopSalesByLocationDto>();

            var provinceCount = deliveredOrders
                .Select(o =>
                {
                    var addressParts = o.shippingAddress.Split(',');
                    // Lấy phần tử thứ hai từ cuối (thường là tỉnh/thành phố)
                    string province = addressParts.Length >= 2
                        ? addressParts[^2].Trim() // ^2 là index từ cuối lên, -2
                        : "Unknown";

                    return province;
                })
                .GroupBy(p => p)
                .Select(g => new TopSalesByLocationDto
                {
                    city = g.Key,
                    totalSold = g.Count() // Đây đang đếm số đơn hàng, không phải số lượng sản phẩm bán ra
                })
                .OrderByDescending(x => x.totalSold)
                .Take(5)
                .ToList();

            return provinceCount;
        }
    }
}