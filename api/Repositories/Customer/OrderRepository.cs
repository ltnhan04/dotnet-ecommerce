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
using MongoDB.Driver;

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
            var userObjectId = ObjectId.Parse(userId);
            var orders = await _context.Orders
                .Where(item => item.user == userObjectId)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                throw new AppException("Get orders failed", 400);
            }

            var variantIds = orders
                .SelectMany(o => o.variants.Select(v => ObjectId.Parse(v.variant.ToString())))
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
                _id = order._id.ToString(),
                user = order.user.ToString(),
                variants = order.variants.Select(v =>
                {
                    var variantData = variantMap.GetValueOrDefault(v.variant.ToString());
                    var productId = variantData?.product;
                    var productData = productMap.GetValueOrDefault(productId!.Value);

                    return new OrderVariantDetail
                    {
                        quantity = v.quantity,
                        variant = new VariantOrderDto
                        {
                            _id = v.variant.ToString(),
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
                status = order.status,
                createdAt = order.createdAt,
                updatedAt = order.updatedAt
            }).ToList();

            return result;
        }

        public async Task<CancelOrderDto> CancelOrder(string orderId)
        {

            var order = await _context.Orders
                .Where(item => item._id == ObjectId.Parse(orderId))
                .FirstOrDefaultAsync();

            if (order == null)
            {
                throw new AppException("Order not found", 404);
            }
            if (order.status != OrderStatus.pending.ToString() && order.status != OrderStatus.processing.ToString())
            {
                throw new AppException("Order cannot be cancelled", 400);
            }
            foreach (var item in order.variants)
            {
                var variantId = item.variant;
                var quantity = item.quantity;

                var variant = await _context.ProductVariants.ToListAsync();
                var match = variant.FirstOrDefault(item => item._id.ToString() == variantId.ToString());

                match.stock_quantity += quantity;
                _context.ProductVariants.Update(match);

                order.status = "cancel";
                await _context.SaveChangesAsync();
            }
            return new CancelOrderDto
            {
                message = "Order cancelled successfully"
            };
        }

        public async Task<UpdateOrderPaymentResponseDto> UpdateOrderPayment(UpdateOrderPaymentDto dto)
        {
            var order = await _context.Orders
                .Where(item => item._id == ObjectId.Parse(dto.orderId))
                .FirstOrDefaultAsync();

            if (order == null)
            {
                throw new AppException("Order not found", 404);
            }

            foreach (var item in order.variants)
            {
                var variantId = item.variant;
                var quantity = item.quantity;

                var variant = await _context.ProductVariants.ToListAsync();
                var match = variant.FirstOrDefault(item => item._id.ToString() == variantId.ToString());
                match.stock_quantity -= quantity;
                _context.ProductVariants.Update(match);
            }

            order.status = "processing";
            order.stripeSessionId = dto.stripeSessionId;
            await _context.SaveChangesAsync();
            return new UpdateOrderPaymentResponseDto
            {
                message = "Order updated successfully"
            };
        }
    }
}