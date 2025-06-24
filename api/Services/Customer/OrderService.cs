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

        public async Task<Order> HandleCreateOrder(OrderCreateDto dto, string userId)
        {
            if (dto.variants == null || !dto.variants.Any())
            {
                throw new AppException("Product variants is required", 400);
            }

            var order = new Order
            {   
                user = userId,
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
            return createOrder;
        }

        public async Task<List<OrderDtoResponse>> HandleGetOrderUser(string userId)
        {
            var orders = await _orderRepository.GetOrderByUser(userId);
            return orders;
        }
    }

}