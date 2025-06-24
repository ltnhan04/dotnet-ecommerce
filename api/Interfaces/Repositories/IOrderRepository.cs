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
        Task<Order> CreateOrder(Order order);
        Task<List<OrderDtoResponse>> GetOrderByUser(string userId);
    }
}