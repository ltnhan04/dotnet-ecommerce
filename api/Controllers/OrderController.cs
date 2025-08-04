using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Interfaces.Repositories;
using api.Models;
using api.Services.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace api.Controllers
{
    [ApiController]
    [Route("v1/orders")]
    [Authorize]
    public class OrderController(IOrderService orderService, INotificationRepository notificationRepository) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly INotificationRepository _notificationRepository = notificationRepository;

        [HttpPost]
        public async Task CreateOrder([FromBody] OrderCreateDto dto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var data = await _orderService.HandleCreateOrder(dto!, userId!);
                if (data != null)
                {
                    await _notificationRepository.Create(new Notification
                    {
                        userId = ObjectId.Parse(userId),
                        isRead = false,
                        title = "📦 Đặt hàng thành công",
                        message = $"Đơn hàng #{data._id} của bạn đang chờ xác nhận.",
                        redirectUrl = "/orders",
                        targetRole = "user",
                        type = "order",
                        createdAt = DateTime.UtcNow,
                        updatedAt = DateTime.UtcNow
                    });
                    await _notificationRepository.Create(new Notification
                    {
                        userId = null,
                        isRead = false,
                        title = "📦 Có đơn hàng mới",
                        message = $"Khách hàng vừa đặt đơn hàng #{data._id}.",
                        redirectUrl = $"/Admin/Orders/Details/{data._id}",
                        targetRole = "admin",
                        type = "order",
                        createdAt = DateTime.UtcNow,
                        updatedAt = DateTime.UtcNow
                    });
                }
                await ResponseHandler.SendSuccess(Response, data, 201, "Create order successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [HttpGet]
        public async Task GetOrderByUser()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var data = await _orderService.HandleGetOrderUser(userId!);
                await ResponseHandler.SendSuccess(Response, data, 200, "Get order successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPut("{orderId}")]
        public async Task CancelOrder(string orderId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var data = await _orderService.HandleCancelOrder(orderId);
                if (data.paymentMethod == "stripe")
                {
                    await _notificationRepository.Create(new Notification
                    {
                        userId = ObjectId.Parse(userId),
                        title = "💰 Đơn hàng đã được hoàn tiền",
                        message = $"Đơn hàng #{data._id} đã được hoàn tiền thành công qua Stripe.",
                        targetRole = "user",
                        type = "refund",
                        isRead = false,
                        redirectUrl = "/orders",
                        createdAt = DateTime.UtcNow
                    });
                }
                if (data.paymentMethod == "momo")
                {
                    await _notificationRepository.Create(new Notification
                    {
                        userId = ObjectId.Parse(userId),
                        title = "💰 Đơn hàng đã được hoàn tiền",
                        message = $"Đơn hàng #{data._id} đã được hoàn tiền thành công qua Momo.",
                        targetRole = "user",
                        type = "refund",
                        isRead = false,
                        redirectUrl = "/orders",
                        createdAt = DateTime.UtcNow
                    });
                }
                if (data.status == "cancel")
                {
                    await _notificationRepository.Create(new Notification
                    {
                        userId = ObjectId.Parse(userId),
                        isRead = false,
                        title = "❌ Đơn hàng bị hủy",
                        message = $"Đơn hàng #{orderId} của bạn đã bị hủy.",
                        redirectUrl = "/orders",
                        targetRole = "user",
                        type = "order",
                        createdAt = DateTime.UtcNow,
                        updatedAt = DateTime.UtcNow
                    });
                    await _notificationRepository.Create(new Notification
                    {
                        userId = null,
                        isRead = false,
                        title = "❌ Khách hủy đơn hàng",
                        message = $"Khách hàng vừa hủy đơn hàng #{orderId}.",
                        redirectUrl = $"/Admin/Orders/Details/{orderId}",
                        targetRole = "admin",
                        type = "order",
                        createdAt = DateTime.UtcNow,
                        updatedAt = DateTime.UtcNow
                    });
                }
                await ResponseHandler.SendSuccess(Response, data, 200, "Cancel order successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPost("update-order-payment")]
        public async Task UpdateOrderPayment([FromBody] UpdateOrderPaymentDto dto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var data = await _orderService.HandleUpdateOrderPayment(dto);
                if (data != null)
                {
                    if (data.status == "cancel")
                    {
                        await _notificationRepository.Create(new Notification
                        {
                            userId = ObjectId.Parse(userId),
                            isRead = false,
                            title = "❌ Đơn hàng bị hủy",
                            message = $"Đơn hàng #{dto.orderId} của bạn đã bị hủy.",
                            redirectUrl = "/orders",
                            targetRole = "customer",
                            type = "order",
                            createdAt = DateTime.UtcNow
                        });
                    }
                    if (data.status == "processing")
                    {
                        await _notificationRepository.Create(new Notification
                        {
                            userId = ObjectId.Parse(userId),
                            isRead = false,
                            title = "✅ Thanh toán thành công",
                            message = $"Bạn đã thanh toán thành công cho đơn hàng #{dto.orderId}.",
                            redirectUrl = "/orders",
                            targetRole = "customer",
                            type = "payment",
                            createdAt = DateTime.UtcNow
                        });
                    }
                }
                await ResponseHandler.SendSuccess(Response, data, 200, "Payment successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}