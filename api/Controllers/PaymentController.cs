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
    [Route("api/v1/payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-checkout-session")]
        public async Task CreateCheckoutSession([FromBody] PaymentDto dto)
        {
            try
            {
                var data = await _paymentService.HandleCreateCheckoutSession(dto.orderId, dto.variants);
                await ResponseHandler.SendSuccess(Response, data, 201, "Created checkout session successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPost("momo/create")]
        public async Task CreateMomoPayment([FromBody] PaymentMomoDto dto)
        {
            try
            {
                var data = await _paymentService.HandleCreateMomoPayment(dto);
                await ResponseHandler.SendSuccess(Response, data, 201, "Created momo successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [AllowAnonymous]
        [HttpPost("momo/callback")]
        public async Task MomoCallback([FromBody] MomoCallbackDto dto)
        {
            try
            {   
                var data = await _paymentService.HandleMomoCallback(dto);
                await ResponseHandler.SendSuccess(Response, data, 200, "Callback momo payment successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}