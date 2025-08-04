using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace api.Controllers
{
    [ApiController]
    [Route("v1/points")]
    [Authorize]
    public class PointController : ControllerBase
    {
        private readonly IPointService _pointService;
        private readonly INotificationRepository _notificationRepository;
        public PointController(IPointService pointService, INotificationRepository notificationRepository)
        {
            _pointService = pointService;
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public async Task GetCustomerPoint()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var data = await _pointService.HandleGetCustomerPoint(userId!);
                await ResponseHandler.SendSuccess(Response, data, 200, "Get customer point successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpGet("vouchers")]
        public async Task GetCustomerVoucher()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var data = await _pointService.HandleGetCustomerVoucher(userId!);
                await ResponseHandler.SendSuccess(Response, data, 200, "Get customer voucher successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPost("exchange-voucher")]
        public async Task ExchangePointForVoucher([FromBody] ExchangePointForVoucherDto dto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var data = await _pointService.HandleExchangePointForVoucher(dto, userId!);
                await _notificationRepository.Create(new Notification
                {
                    userId = ObjectId.Parse(userId),
                    title = "🎁 Nhận được voucher mới",
                    message = $"Bạn vừa đổi điểm lấy mã giảm giá {data.code}",
                    type = "promotion",
                    targetRole = "user",
                    isRead = false,
                    redirectUrl = "/exchange-voucher",
                    createdAt = DateTime.UtcNow
                });
                await ResponseHandler.SendSuccess(Response, data, 200, "Exchange point for voucher successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPost("apply-voucher")]
        public async Task ApplyVoucher([FromBody] ApplyVoucherDto dto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var data = await _pointService.HandleApplyVoucher(dto, userId!);
                await ResponseHandler.SendSuccess(Response, data, 200, "Applied voucher successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPut("status-voucher")]
        public async Task UpdateStatusVoucher([FromBody] UpdateStatusVoucherDto dto)
        {
            try
            {
                var data = await _pointService.HandleUpdateStatusVoucher(dto);
                await ResponseHandler.SendSuccess(Response, data, 200, "Updated status voucher successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}