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
    }
}