using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.Repositories;
using api.models;
using api.Models;
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
        public async Task<List<Notification>> GetByUserIdOrRole(string userId, string role)
        {
            var userObjectId = ObjectId.Parse(userId);
            return await _context.Notifications
                .Where(n => n.userId == userObjectId || n.userId == null || n.targetRole == role)
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
    }
}