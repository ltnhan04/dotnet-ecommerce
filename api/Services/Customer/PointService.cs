using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using api.Dtos;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.models;
using api.Utils;
using CloudinaryDotNet.Actions;
using MongoDB.Bson;

namespace api.Services.Customer
{
    public class PointService : IPointService
    {
        private readonly IPointRepository _pointRepository;
        public PointService(IPointRepository pointRepository)
        {
            _pointRepository = pointRepository;
        }

        public async Task<GetCustomerPointDto> HandleGetCustomerPoint(string userId)
        {
            var data = await _pointRepository.GetCustomerPoint(userId);
            var points = data.Sum(item => item.points);

            return new GetCustomerPointDto
            {
                points = points
            };
        }

        public async Task<List<GetCustomerVoucherDto>> HandleGetCustomerVoucher(string userId)
        {
            var data = await _pointRepository.GetCustomerVoucher(userId);
            var vouchers = data.Select(item => new VoucherDto
            {
                _id = item._id.ToString(),
                code = item.code,
                discountAmount = (int)item.discountAmount,
                pointsUsed = item.pointsUsed,
                status = item.status,
                validFrom = item.validFrom,
                validTo = item.validTo
            }).ToList();

            return new List<GetCustomerVoucherDto>
            {
                new GetCustomerVoucherDto
                {
                    vouchers = vouchers
                }
            };
        }

        public async Task<VoucherDto> HandleExchangePointForVoucher(ExchangePointForVoucherDto dto, string userId)
        {
            var pointAvailable = await _pointRepository.GetCustomerPoint(userId);
            var currentPoint = pointAvailable.Sum(p => p.points);

            if (currentPoint < dto.pointsToUse)
            {
                throw new AppException("Insufficient points", 400);
            }

            var discount = new Dictionary<int, int>
            {
                {100, 100000},
                {300, 500000},
                {500, 10000000}
            };

            var discountAmount = discount[dto.pointsToUse];
            var voucher = new PointVoucher
            {
                _id = ObjectId.GenerateNewId(),
                customer = ObjectId.Parse(userId),
                code = $"POINT- {Guid.NewGuid().ToString().Substring(0,8).ToLower()}",
                discountAmount = discountAmount,
                pointsUsed = dto.pointsToUse,
                status = "unused",
                validFrom = DateTime.Now,
                validTo = DateTime.Now.AddDays(30)
            };

            await _pointRepository.ExchangePointForVoucher(voucher);
            return new VoucherDto
            {
                _id = voucher._id.ToString(),
                customer = voucher.customer.ToString(),
                code = voucher.code,
                discountAmount = (int)voucher.discountAmount,
                pointsUsed = voucher.pointsUsed,
                status = voucher.status,
                validFrom = voucher.validFrom,
                validTo = voucher.validTo
            };
        }

        public async Task<models.Point> HandleAddPointForOrder(Order order)
        {
            var points = Math.Round((double)order.totalAmount / 100000);
            if (points <= 0)
            {
                throw new AppException("Error points order", 400);
            }

            var expiryDate = DateTime.Now.AddMonths(3);

            var point = new models.Point
            {
                _id = ObjectId.GenerateNewId(),
                customer = order.user,
                points = (int)points,
                order = order._id,
                expiryDate = expiryDate,
                isExpired = false,
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            };

            await _pointRepository.AddPointForOrder(point);
            return point;
        }
    }
}