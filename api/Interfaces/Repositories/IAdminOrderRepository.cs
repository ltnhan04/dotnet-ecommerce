using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;

namespace api.Interfaces.Repositories
{
    public interface IAdminOrderRepository
    {
        Task<PaginateDto<AdminGetAllOrder>> GetAllOrder(int page, int size);
        Task<AdminGetAllOrder> GetOrderDetail(string orderId);
        Task<AdminResponseUpdateOrderStatus> UpdateOrderStatus(string orderId, stateDto dto);
    }
}