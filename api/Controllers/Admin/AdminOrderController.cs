using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.Models;
using api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace api.Controllers.Admin
{
    [ApiController]
    [Route("v1/admin/orders")]
    [Authorize(Roles = "admin")]
    public class AdminOrderController : ControllerBase
    {
        private readonly IAdminOrderService? _orderService;
        private readonly INotificationRepository _notificationRepository;

        public AdminOrderController(IAdminOrderService orderService, INotificationRepository notificationRepository)
        {
            _orderService = orderService;
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public async Task GetAllOrder([FromQuery] GetOrderQueryDto dto)
        {
            try
            {
                var data = await _orderService.HandleGetAllOrder(dto);
                await ResponseHandler.SendSuccess(Response, data, 200, "Get all orders successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpGet("{orderId}")]
        public async Task GetOrderDetail(string orderId)
        {
            try
            {
                var data = await _orderService.HandleGetOrderDetail(orderId);
                await ResponseHandler.SendSuccess(Response, data, 200, "Get order detail successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPut("{orderId}")]
        public async Task UpdateOrderStatus(string orderId, [FromBody] stateDto dto)
        {
            try
            {
                var data = await _orderService.HandleUpdateOrderStatus(orderId, dto);
                if (data != null)
                {
                    var (userTitle, userMessage, adminTitle, adminMessage) = GetNotificationForOrderStatus.GetNotifications(data.status, orderId);
                    await _notificationRepository.Create(new Notification
                    {
                        userId = ObjectId.Parse(data.user),
                        isRead = false,
                        title = userTitle,
                        message = userMessage,
                        redirectUrl = "/orders",
                        targetRole = "user",
                        type = "order",
                        createdAt = DateTime.UtcNow,
                        updatedAt = DateTime.UtcNow,
                    });

                    await _notificationRepository.Create(new Notification
                    {
                        userId = null,
                        isRead = false,
                        title = adminTitle,
                        message = adminMessage,
                        redirectUrl = $"/admin/orders/details/{orderId}",
                        targetRole = "admin",
                        type = "order",
                        createdAt = DateTime.UtcNow,
                        updatedAt = DateTime.UtcNow,
                    });
                }
                await ResponseHandler.SendSuccess(Response, data, 200, "Updated order status successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}