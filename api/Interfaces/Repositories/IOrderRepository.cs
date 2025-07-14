using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;

namespace api.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderById(string orderId);
        Task<Order> CreateOrder(Order order);
        Task<List<OrderDtoResponse>> GetOrderByUser(string userId);
        Task<CancelOrderDto> CancelOrder(string orderId);
        Task<UpdateOrderPaymentResponseDto> UpdateOrderPayment(UpdateOrderPaymentDto dto);
        Task<Order> UpdateOrder(Order order);
        Task<ProductVariant?> GetVariantById(string variantId);
        Task<ProductVariant> UpdateVariant(ProductVariant variant);

    }
}