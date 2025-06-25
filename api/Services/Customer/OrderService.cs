using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.models;
using api.Utils;
using MongoDB.Bson;

namespace api.Services.Customer
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderCreateDto> HandleCreateOrder(OrderCreateDto dto, string userId)
        {
            if (dto.variants == null || !dto.variants.Any())
            {
                throw new AppException("Product variants is required", 400);
            }

            var order = new Order
            {
                user = ObjectId.Parse(userId),
                variants = dto.variants.Select(item => new OrderVariant
                {
                    variant = ObjectId.Parse(item.variant),
                    quantity = item.quantity
                }).ToList(),
                totalAmount = dto.totalAmount,
                shippingAddress = dto.shippingAddress,
                paymentMethod = dto.paymentMethod,
                status = "pending"
            };

            var createOrder = await _orderRepository.CreateOrder(order) ?? throw new AppException("Failed to create order", 400);
            return new OrderCreateDto
            {
                _id = createOrder._id.ToString(),
                variants = createOrder.variants.Select(v => new OrderCreateVariantDetail
                {
                    variant = v.variant.ToString(),
                    quantity = v.quantity
                }).ToList(),
                createdAt = createOrder.createdAt,
                totalAmount = createOrder.totalAmount,
                shippingAddress = createOrder.shippingAddress,
                status = createOrder.status,
                paymentMethod = createOrder.paymentMethod
            };
        }

        public async Task<List<OrderDtoResponse>> HandleGetOrderUser(string userId)
        {
            var orders = await _orderRepository.GetOrderByUser(userId);
            return orders;
        }

        public async Task<CancelOrderDto> HandleCancelOrder(string orderId)
        {
            var orders = await _orderRepository.CancelOrder(orderId);
            return orders;
        }

        public async Task<UpdateOrderPaymentResponseDto> HandleUpdateOrderPayment(UpdateOrderPaymentDto dto)
        {
            if (string.IsNullOrEmpty(dto.orderId) || string.IsNullOrEmpty(dto.stripeSessionId))
            {
                throw new AppException("Missing orderId or sessionId", 400);
            }

            var stripeUtil = new StripeUtil();
            var sessionStripe = await stripeUtil.GetSessionAsync(dto.stripeSessionId);
            if (sessionStripe == null || sessionStripe.PaymentStatus != "paid")
            {
                throw new AppException("Payment not successful", 400);
            }

            var data = await _orderRepository.UpdateOrderPayment(dto);
            return data;
        }
    }

}