using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;

namespace api.Interfaces.Services
{
    public interface IAdminOrderService
    {
        Task<PaginateDto<AdminGetAllOrder>> HandleGetAllOrder(GetOrderQueryDto dto);
        Task<AdminGetAllOrder> HandleGetOrderDetail(string orderId);
        Task<AdminResponseUpdateOrderStatus> HandleUpdateOrderStatus(string orderId, stateDto dto);
    }
}