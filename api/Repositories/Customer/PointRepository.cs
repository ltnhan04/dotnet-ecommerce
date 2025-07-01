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
    }
}