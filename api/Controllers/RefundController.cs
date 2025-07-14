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
    [Route("api/v1/refunds")]
    [Authorize]
    public class RefundController : ControllerBase
    {
        private readonly IRefundService _refundService;
        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }
        [HttpPost("stripe")]
        public async Task RefundStripe(RefundDto dto)
        {
            try
            {
                var refundId = await _refundService.HandleStripeRefund(dto.orderId, dto.reason);
                await ResponseHandler.SendSuccess(Response, null, 200, "Refund successful");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}