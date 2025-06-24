using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.models;
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

        public async Task<Order> HandleCreateOrder(OrderDto dto, string userId)
        {
            if (dto.variants == null || !dto.variants.Any())
            {
                throw new Exception("Product variants is required");
            }

            var order = new Order
            {   
                user = ObjectId.Parse(userId),
                variants = dto.variants.Select(item => new models.OrderVariant
                {
                    variant = ObjectId.Parse(item.variant),
                    quantity = item.quantity
                }).ToList(),
                totalAmount = dto.totalAmount,
                shippingAddress = dto.shippingAddress,
                paymentMethod = dto.paymentMethod,
                status = "pending"
            };

            var createOrder = await _orderRepository.CreateOrder(order)
                              ?? throw new Exception("Failed to create order");

            return createOrder;
        }
    }

}