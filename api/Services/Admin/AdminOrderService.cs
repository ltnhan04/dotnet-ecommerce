using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.models;
using api.Utils;

namespace api.Services.Admin
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly IAdminOrderRepository? _orderRepository;
        public AdminOrderService(IAdminOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<PaginateDto<AdminGetAllOrder>> HandleGetAllOrder(int page, int size)
        {
            return await _orderRepository.GetAllOrder(page, size);
        }

        public async Task<AdminGetAllOrder> HandleGetOrderDetail(string orderId)
        {
            return await _orderRepository.GetOrderDetail(orderId);
        }

        public async Task<AdminResponseUpdateOrderStatus> HandleUpdateOrderStatus(string orderId, stateDto dto)
        {
            var validStatus = new List<string>
            {
                "pending",
                "processing",
                "shipped",
                "delivered",
                "cancelled"
            };
            if (!validStatus.Contains(dto.status))
            {
                throw new AppException("Invalid status", 400);
            }
            var data = await _orderRepository.UpdateOrderStatus(orderId, dto) ?? throw new AppException("Order not found", 404);
            return data;
        }
    }
}