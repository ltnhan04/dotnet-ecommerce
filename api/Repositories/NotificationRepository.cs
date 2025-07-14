using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.models;
using api.Models;
using api.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace api.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly iTribeDbContext _context;
        public NotificationRepository(iTribeDbContext context)
        {
            _context = context;
        }
        public async Task<List<Notification>> GetByUserIdOrRole(string userId, string role, string type = "all")
        {
            var userObjectId = ObjectId.Parse(userId);
            var query = _context.Notifications
                .Where(n => n.userId == userObjectId || n.targetRole == role);

            if (type != "all")
            {
                query = query.Where(n => n.type == type);
            }

            return await query
                .OrderByDescending(n => n.createdAt)
                .ToListAsync();
        }
        public async Task<bool> MarkAsRead(string id)
        {
            var notification = await _context.Notifications.FindAsync(ObjectId.Parse(id));
            if (notification == null) return false;

            notification.isRead = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task Create(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> AlreadySentMilestoneNotification(string userId, int milestone)
        {
            return await _context.Notifications.AnyAsync(n =>
                n.userId == ObjectId.Parse(userId) &&
                n.type == "promotion" &&
                n.message.Contains($"mốc {milestone}")
            );
        }
    }
}
