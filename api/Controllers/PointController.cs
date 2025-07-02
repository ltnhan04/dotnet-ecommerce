using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/points")]
    [Authorize]
    public class PointController : ControllerBase
    {
        private readonly IPointService _pointService;
        public PointController(IPointService pointService)
        {
            _pointService = pointService;
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