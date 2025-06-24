using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.models;
using api.Utils;
using MongoDB.Bson;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories.Customer
{
    public class OrderRepository : IOrderRepository
    {
        private readonly iTribeDbContext _context;
        public OrderRepository(iTribeDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<OrderDtoResponse>> GetOrderByUser(string userId)
        {
            var orders = await _context.Orders
                .Where(item => item.user == userId)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                throw new AppException("Get orders failed", 400);
            }

            var variantIds = orders
                .SelectMany(o => o.variants.Select(v => ObjectId.Parse(v.variant)))
                .Distinct()
                .ToList();

            var variants = await _context.ProductVariants
                .Where(pv => variantIds.Contains(pv._id))
                .ToListAsync();

            var variantMap = variants.ToDictionary(v => v._id.ToString(), v => v);

            var productIds = variants
                .Select(v => v.product)
                .Distinct()
                .ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p._id))
                .ToListAsync();

            var productMap = products.ToDictionary(p => p._id, p => p);

            var result = orders.Select(order => new OrderDtoResponse
            {
                _id = order._id,
                user = order.user.ToString(),
                variants = order.variants.Select(v =>
                {
                    var variantData = variantMap.GetValueOrDefault(v.variant);
                    var productId = variantData?.product;
                    var productData = productMap.GetValueOrDefault(productId!.Value);

                    return new OrderVariantDetail
                    {
                        quantity = v.quantity,
                        variant = new VariantOrderDto
                        {
                            _id = v.variant,
                            product = productId.ToString()!,
                            productName = productData?.name ?? "null",
                            colorName = variantData?.color.colorName!,
                            colorCode = variantData?.color.colorCode!,
                            stock_quantity = variantData?.stock_quantity ?? 0,
                            storage = variantData?.storage!,
                            price = variantData?.price ?? 0,
                            images = variantData?.images ?? new List<string>()
                        }
                    };
                }).ToList(),
                totalAmount = order.totalAmount,
                shippingAddress = order.shippingAddress,
                paymentMethod = order.paymentMethod,
                stripeSessionId = order.stripeSessionId!,
                createdAt = order.createdAt,
                updatedAt = order.updatedAt
            }).ToList();

            return result;
        }

    }
}