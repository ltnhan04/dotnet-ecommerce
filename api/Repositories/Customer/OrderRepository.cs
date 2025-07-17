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
using Microsoft.AspNetCore.Http.Features;
using api.Interfaces.Repositories;

namespace api.Repositories.Customer
{
    public class OrderRepository : IOrderRepository
    {
        private readonly iTribeDbContext _context;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IAdminOrderRepository _adminOrderRepository;

        public OrderRepository(iTribeDbContext context, IProductVariantRepository productVariantRepository, IAdminOrderRepository adminOrderRepository)
        {
            _context = context;
            _productVariantRepository = productVariantRepository;
            _adminOrderRepository = adminOrderRepository;
        }
        public async Task<Order?> GetOrderById(string orderId)
        {
            var orders = await _context.Orders.ToListAsync();
            var order = orders.FirstOrDefault(o => o._id.ToString() == orderId);
            return order;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<Order> UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<OrderDtoResponse>> GetOrderByUser(string userId)
        {
            var userObjectId = ObjectId.Parse(userId);
            var orders = await _context.Orders
                .Where(item => item.user == userObjectId)
                .ToListAsync();

            if (orders == null || orders.Count == 0)
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
            var orders = await _context.Orders.ToListAsync();
            var order = orders.FirstOrDefault(item => item._id == ObjectId.Parse(orderId));
            if (order.status != OrderStatus.pending.ToString() && order.status != OrderStatus.processing.ToString())
                throw new AppException("Order cannot be cancelled", 400);
            var variantIds = order.variants.Select(v => v.variant).ToList();
            var variants = await _context.ProductVariants
                .Where(v => variantIds.Contains(v._id))
                .ToListAsync();

            foreach (var item in order.variants)
            {
                var variant = variants.FirstOrDefault(v => v._id == item.variant);
                if (variant == null) continue;

                variant.stock_quantity += item.quantity;
                _context.ProductVariants.Update(variant);

                await _productVariantRepository.CheckVariantLowStock(variant._id.ToString());
            }

            order.status = "cancel";
            order.updatedAt = DateTime.UtcNow;
            _context.Orders.Update(order);

            await _context.SaveChangesAsync();

            var productIds = variants.Select(v => v.product).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p._id))
                .ToListAsync();

            var variantDict = variants.ToDictionary(v => v._id, v => v);
            var productDict = products.ToDictionary(p => p._id, p => p);

            return new CancelOrderDto
            {
                _id = order._id.ToString(),
                user = order.user.ToString(),
                variants = order.variants.Select(v => new OrderVariantDetail
                {
                    quantity = v.quantity,
                    variant = new VariantOrderDto
                    {
                        _id = v.variant.ToString(),
                        product = variantDict[v.variant].product.ToString(),
                        productName = productDict[variantDict[v.variant].product].name,
                        colorName = variantDict[v.variant].color.colorName,
                        colorCode = variantDict[v.variant].color.colorCode,
                        storage = variantDict[v.variant].storage,
                        price = variantDict[v.variant].price,
                        images = variantDict[v.variant].images ?? new List<string>()
                    }
                }).ToList(),
                totalAmount = order.totalAmount,
                paymentMethod = order.paymentMethod,
                status = order.status,
                shippingAddress = order.shippingAddress,
                isPaymentMomo = order.isPaymentMomo,
                stripeSessionId = order.stripeSessionId ?? "null",
                createdAt = order.createdAt,
                updatedAt = order.updatedAt
            };
        }

        public async Task<UpdateOrderPaymentResponseDto> UpdateOrderPayment(UpdateOrderPaymentDto dto)
        {
            var orders = await _context.Orders.ToListAsync();
            var order = orders.FirstOrDefault(item => item._id == ObjectId.Parse(dto.orderId));

            foreach (var item in order.variants)
            {
                await _productVariantRepository.CheckVariantLowStock(item.variant.ToString());
            }
            if (order.status == "pending")
            {
                await _adminOrderRepository.UpdateOrderStatus(order._id.ToString(), new stateDto { status = "processing" });
            }
            order.stripeSessionId = dto.stripeSessionId;
            await _context.SaveChangesAsync();

            var variantIds = order.variants.Select(item => item.variant).ToList();
            var variants = await _context.ProductVariants
                .Where(item => variantIds.Contains(item._id))
                .ToListAsync();

            var productIds = variants.Select(item => item.product).Distinct().ToList();
            var products = await _context.Products
                .Where(item => productIds.Contains(item._id))
                .ToListAsync();

            var variantDict = variants.ToDictionary(item => item._id, item => item);
            var productDict = products.ToDictionary(item => item._id, item => item);
            return new UpdateOrderPaymentResponseDto
            {
                _id = order._id.ToString(),
                user = order.user.ToString(),
                variants = order.variants.Select(v => new OrderVariantDetail
                {
                    quantity = v.quantity,
                    variant = new VariantOrderDto
                    {
                        _id = v.variant.ToString(),
                        product = variantDict[v.variant].product.ToString(),
                        productName = productDict[variantDict[v.variant].product].name,
                        colorName = variantDict[v.variant].color.colorName,
                        colorCode = variantDict[v.variant].color.colorCode,
                        storage = variantDict[v.variant].storage,
                        price = variantDict[v.variant].price,
                        images = variantDict[v.variant].images ?? new List<string>(),
                    }
                }).ToList(),
                totalAmount = order.totalAmount,
                paymentMethod = order.paymentMethod,
                stripeSessionId = order.stripeSessionId,
                status = order.status,
                shippingAddress = order.shippingAddress,
                createdAt = order.createdAt,
                updatedAt = order.updatedAt
            };
        }

        public async Task<ProductVariant?> GetVariantById(string variantId)
        {
            var objectId = ObjectId.Parse(variantId);
            var variants = await _context.ProductVariants.ToListAsync();
            return variants.FirstOrDefault(v => v._id == objectId);
        }

        public async Task<ProductVariant> UpdateVariant(ProductVariant variant)
        {
            _context.ProductVariants.Update(variant);
            await _context.SaveChangesAsync();
            return variant;
        }

    }
}