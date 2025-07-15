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
        private readonly INotificationService _notificationService;
        public PointService(IPointRepository pointRepository, INotificationService notificationService)
        {
            _pointRepository = pointRepository;
            _notificationService = notificationService;
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

        public async Task<List<VoucherDto>> HandleGetCustomerVoucher(string userId)
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

            return vouchers;
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
                {500, 1000000}
            };

            var discountAmount = discount[dto.pointsToUse];
            var voucher = new PointVoucher
            {
                _id = ObjectId.GenerateNewId(),
                customer = ObjectId.Parse(userId),
                code = $"POINT-{Guid.NewGuid().ToString().Substring(0, 8).ToLower()}",
                discountAmount = discountAmount,
                pointsUsed = dto.pointsToUse,
                status = "unused",
                validFrom = DateTime.Now,
                validTo = DateTime.Now.AddDays(30)
            };

            await _pointRepository.ExchangePointForVoucher(voucher);
            var points = await _pointRepository.DeductPoint(userId);
            var pointDeduct = dto.pointsToUse;
            var pointUpdate = new List<models.Point>();
            var pointDelete = new List<models.Point>();
            foreach (var item in points)
            {
                if (pointDeduct <= 0) break;
                if (item.points <= pointDeduct)
                {
                    pointDeduct -= item.points;
                    pointDelete.Add(item);

                }
                else
                {
                    item.points -= pointDeduct;
                    pointUpdate.Add(item);
                    pointDeduct = 0;
                }
            }

            if (pointUpdate.Count > 0) await _pointRepository.UpdatePoint(pointUpdate);
            if (pointDelete.Count > 0) await _pointRepository.DeletePoint(pointDelete);
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
            await _notificationService.NotifyNearMilestone(order.user.ToString());
            return point;
        }

        public async Task<ApplyVoucherResponseDto> HandleApplyVoucher(ApplyVoucherDto dto, string userId)
        {
            var data = await _pointRepository.GetValidVoucher(dto, userId) ?? throw new AppException("Invalid or expired voucher", 400);
            var discountedTotal = Math.Max(dto.orderTotal - data.discountAmount, 0);
            return new ApplyVoucherResponseDto
            {
                voucherCode = data.code.ToString(),
                discountAmount = (int)data.discountAmount,
                discountedTotal = (int)discountedTotal
            };
        }

        public async Task<PointVoucherDto> HandleUpdateStatusVoucher(UpdateStatusVoucherDto dto)
        {   
            
            var data = await _pointRepository.UpdateStatusVoucher(dto) ?? throw new AppException("Voucher invalid or already used", 400);
            return new PointVoucherDto
            {
                _id = data._id.ToString(),
                code = data.code,
                customer = data.customer.ToString(),
                discountAmount = (int)data.discountAmount,
                pointsUsed = data.pointsUsed,
                status = data.status,
                usedOrder = data.usedOrder.ToString(),
                validFrom = data.validFrom,
                validTo = data.validTo
            };
        }

        public async Task<VoucherFreeShipDto> CreateFirstOrderFreeShipPromotion(string customerId)
        {
            await _pointRepository.CheckFirstOrderOfCustomer(customerId);
            var code = "FREESHIP" + "-" + ObjectId.GenerateNewId(16);
            var voucher = new models.PointVoucher
            {
                _id = ObjectId.GenerateNewId(),
                customer = ObjectId.Parse(customerId),
                code = code,
                usedOrder = null,
                discountAmount = 30000,
                validFrom = DateTime.UtcNow,
                validTo = DateTime.UtcNow.AddDays(30),
                pointsUsed = 0,
                status = "unused",
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            };
            await _pointRepository.ExchangePointForVoucher(voucher);
            Console.WriteLine("VOUCHER: " + voucher);
            return new VoucherFreeShipDto
            {
                _id = voucher._id.ToString(),
                customer = voucher.customer.ToString(),
                code = voucher.code,
                discountAmount = (int)voucher.discountAmount,
                pointsUsed = voucher.pointsUsed,
                status = voucher.status,
                usedOrder = voucher.usedOrder.ToString(),
                validFrom = voucher.validFrom,
                validTo = voucher.validTo
            };
        }
    }
}