using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace api.Interfaces
{
    public interface IOrderService
    {
        Task<Order> HandleCreateOrder(OrderCreateDto order, string userId);
        Task<List<OrderDtoResponse>> HandleGetOrderUser(string userId);

    }
}