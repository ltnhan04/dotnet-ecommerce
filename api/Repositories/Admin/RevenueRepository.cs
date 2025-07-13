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

        public async Task<TotalDto> GetTotalDashboardData(DateTime? fromDate = null, DateTime? toDate = null)
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

            var orders = await query.ToListAsync();

            var totalOrder = orders.Count();
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

        public async Task<List<RevenueDto>> GetRevenueChartData(DateTime fromDate, DateTime toDate, string granularity)
        {
            fromDate = fromDate.Date;
            toDate = toDate.Date.AddDays(1).AddTicks(-1);

            var orders = await _context.Orders
                .Where(o => o.status == "delivered" && o.createdAt >= fromDate && o.createdAt <= toDate)
                .ToListAsync();

            List<RevenueDto> result = new List<RevenueDto>();

            switch (granularity.ToLower())
            {
                case "hourly":
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
                                label = currentPointTime.ToString("HH:mm"), 
                                totalRevenue = totalRevenue
                            });
                        }
                    }
                    break;

                case "daily":
                    for (DateTime date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
                    {
                        var nextDay = date.AddDays(1);
                        var totalRevenue = orders
                            .Where(o => o.createdAt >= date && o.createdAt < nextDay)
                            .Sum(o => o.totalAmount);

                        result.Add(new RevenueDto
                        {
                            label = date.ToString("dd/MM"),
                            totalRevenue = totalRevenue
                        });
                    }
                    break;

                case "weekly":
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
                            label = $"{currentWeekStart:dd/MM} - {nextWeekStart.AddDays(-1):dd/MM}",
                            totalRevenue = totalRevenue
                        });
                        currentWeekStart = nextWeekStart;
                    }
                    break;

                case "monthly":
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
                            label = currentMonth.ToString("MM/yyyy"),
                            totalRevenue = totalRevenue
                        });
                        currentMonth = nextMonth;
                    }
                    break;

                case "yearly":
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
                            label = year.ToString(), 
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
                    item.image = variant.images.Count >= 4 ? variant.images[3] : variant.images.FirstOrDefault() ?? string.Empty;
                    item.price = variant.price;

                    var product = products.FirstOrDefault(p => p._id.ToString() == variant.product.ToString());
                    item.productName = (product?.name + " " + variant.color.colorName + " " + variant.storage) ?? "Unknown";
                }
            }

            return listVariants;
        }

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