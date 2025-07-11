using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;

namespace api.Interfaces.Repositories
{
    public interface IPointRepository
    {
        Task<List<Point>> GetCustomerPoint(string userId);
        Task<List<PointVoucher>> GetCustomerVoucher(string userId);
        Task ExchangePointForVoucher(PointVoucher voucher);
        Task AddPointForOrder(Point point);
        Task<List<Point>> DeductPoint(string customerId);
        Task UpdatePoint(List<Point> point);
        Task DeletePoint(List<Point> point);
        Task<PointVoucher> GetValidVoucher(ApplyVoucherDto dto, string userId);
        Task<PointVoucher> UpdateStatusVoucher(UpdateStatusVoucherDto dto);
        Task<Order?> CheckFirstOrderOfCustomer(string customerId);

    }
}