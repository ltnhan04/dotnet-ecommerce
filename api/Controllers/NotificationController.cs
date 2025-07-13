using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Interfaces.Repositories;
using api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Dtos;
using api.Models;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _repo;

        public NotificationController(INotificationRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task GetAll([FromQuery] string type = "all")
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var notifications = await _repo.GetByUserIdOrRole(userId!, role!, type);

                var notificationDtos = notifications.Select(n => new NotificationDto
                {
                    _id = n._id.ToString(),
                    userId = n.userId?.ToString(),
                    targetRole = n.targetRole,
                    type = n.type,
                    title = n.title,
                    message = n.message ?? "",
                    isRead = n.isRead,
                    redirectUrl = n.redirectUrl,
                    createdAt = n.createdAt,
                    updatedAt = n.updatedAt
                }).ToList();

                await ResponseHandler.SendSuccess(Response, notificationDtos, 200, "Get all notifications successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task MarkAsRead(string id)
        {
            try
            {
                var success = await _repo.MarkAsRead(id);
                if (!success)
                {
                    throw new AppException("Notification not found", 404);
                }
                await ResponseHandler.SendSuccess(Response, null, 200, "Mark as read");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}