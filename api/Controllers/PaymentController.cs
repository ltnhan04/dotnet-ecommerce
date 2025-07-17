using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly INotificationRepository _notificationRepository;

        public PaymentController(IPaymentService paymentService, INotificationRepository notificationRepository)
        {
            _paymentService = paymentService;
            _notificationRepository = notificationRepository;
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
        [HttpPost("momo/check-order-status")]
        public async Task CheckOrderStatusMomo([FromBody] CheckOrderStatusMomo dto)
        {   
            try
            {
                var data = await _paymentService.HandleCheckOrderStatusMomo(dto);
                await ResponseHandler.SendSuccess(Response, data, 201, "Check order status momo successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPost("momo/callback")]
        public async Task MomoCallback([FromBody] MomoCallbackDto dto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var data = await _paymentService.HandleMomoCallback(dto);
                await _notificationRepository.Create(new Notification
                {
                    userId = ObjectId.Parse(userId),
                    title = "✅ Thanh toán thành công",
                    message = $"Bạn đã thanh toán thành công đơn hàng #{data._id}.",
                    type = "payment",
                    targetRole = "user",
                    isRead = false,
                    redirectUrl = $"/orders",
                    createdAt = DateTime.UtcNow
                });

                await _notificationRepository.Create(new Notification
                {
                    title = "💰 Đơn hàng đã được thanh toán",
                    message = $"Khách hàng đã thanh toán đơn hàng #{data._id}.",
                    type = "payment",
                    targetRole = "admin",
                    isRead = false,
                    redirectUrl = $"/Admin/Orders/Details/{data._id}",
                    createdAt = DateTime.UtcNow
                });
                await ResponseHandler.SendSuccess(Response, data, 200, "Callback momo payment successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}