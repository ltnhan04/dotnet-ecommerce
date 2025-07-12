using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.models;
using api.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace api.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly EmailService _emailService;
        private readonly IPointRepository _pointRepository;
        private readonly iTribeDbContext _context;

        public NotificationService(
            INotificationRepository notificationRepository,
            EmailService emailService,
            iTribeDbContext context,
            IPointRepository pointRepository
        )
        {
            _notificationRepository = notificationRepository;
            _emailService = emailService;
            _pointRepository = pointRepository;
            _context = context;
        }

        public async Task NotifyNearMilestone(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u._id == ObjectId.Parse(userId));
            if (user == null || user.role != "user") return;

            var pointsList = await _pointRepository.GetCustomerPoint(userId);
            var totalPoints = pointsList.Sum(p => p.points);

            var milestones = new[] { 100, 300, 500 };
            var threshold = 0.9;

            foreach (var milestone in milestones)
            {
                var notifyAt = (int)(milestone * threshold);
                if (totalPoints >= notifyAt && totalPoints < milestone)
                {
                    var alreadyNotified = await _notificationRepository
                        .AlreadySentMilestoneNotification(user._id.ToString(), milestone);
                    if (alreadyNotified) break;

                    var message = $"Bạn đã tích được {totalPoints} điểm. " +
                                  $"Chỉ còn {milestone - totalPoints} điểm nữa là nhận được voucher mốc {milestone} điểm!";

                    await _notificationRepository.Create(new Notification
                    {
                        userId = user._id,
                        title = "🎯 Gần đủ điểm đổi voucher",
                        message = message,
                        targetRole = "user",
                        type = "promotion",
                        isRead = false,
                        redirectUrl = "/points",
                        createdAt = DateTime.UtcNow
                    });

                    await _emailService.SendMilestoneReminderEmail(user.email, user.name, totalPoints, milestone);
                    break;
                }
            }
        }
    }
}