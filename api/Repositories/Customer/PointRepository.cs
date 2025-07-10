using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.models;
using api.Utils;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace api.Repositories.Customer
{
    public class PointRepository : IPointRepository
    {
        private readonly iTribeDbContext _context;
        public PointRepository(iTribeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Point>> GetCustomerPoint(string userId)
        {
            var points = await _context.Points
                .Where(item => item.customer == ObjectId.Parse(userId) &&
                        !item.isExpired &&
                            item.expiryDate > DateTime.Now)
                .ToListAsync();

            return points;
        }

        public async Task<List<PointVoucher>> GetCustomerVoucher(string userId)
        {
            var vouchers = await _context.PointVouchers
                .Where(item => item.customer == ObjectId.Parse(userId) &&
                        item.status == "unused" &&
                            item.validTo >= DateTime.Now)
                .ToListAsync();

            return vouchers;
        }

        public async Task ExchangePointForVoucher(PointVoucher voucher)
        {
            _context.PointVouchers.Add(voucher);
            await _context.SaveChangesAsync();
        }

        public async Task AddPointForOrder(Point point)
        {
            _context.Points.Add(point);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Point>> DeductPoint(string customerId)
        {
            var points = await _context.Points
                .Where(item => item.customer == ObjectId.Parse(customerId) &&
                        !item.isExpired &&
                            item.expiryDate > DateTime.Now)
                .OrderBy(item => item.expiryDate)
                .ToListAsync();

            return points;
        }

        public async Task UpdatePoint(List<Point> point)
        {
            _context.Points.UpdateRange(point);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePoint(List<Point> point)
        {
            _context.Points.RemoveRange(point);
            await _context.SaveChangesAsync();
        }

        public async Task<PointVoucher> GetValidVoucher(ApplyVoucherDto dto, string userId)
        {
            var voucher = await _context.PointVouchers
                .Where(item => item.code == dto.voucherCode &&
                        item.customer == ObjectId.Parse(userId) &&
                            item.status == "unused" &&
                                item.validTo >= DateTime.Now)
                .FirstOrDefaultAsync();

            return voucher!;
        }

        public async Task<PointVoucher> UpdateStatusVoucher(UpdateStatusVoucherDto dto)
        {
            var pointVoucher = await _context.PointVouchers
                .Where(item => item.code == dto.voucherCode &&
                        item.status == "unused")
                .FirstOrDefaultAsync();

            pointVoucher.status = "used";
            pointVoucher.usedOrder = ObjectId.Parse(dto.orderId.ToString());
            await _context.SaveChangesAsync();

            return pointVoucher;
        }
        public async Task<Order?> CheckFirstOrderOfCustomer(string customerId)
        {
            var orders = await _context.Orders.ToListAsync();
            var order = orders.FirstOrDefault((o) => o.user == ObjectId.Parse(customerId));
            return order;
        }
    }
}