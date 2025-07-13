using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Services;

namespace api.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdOrRole(string userId, string role, string type = "all");
        Task<bool> MarkAsRead(string id);
        Task Create(Notification notification);
        Task<bool> AlreadySentMilestoneNotification(string userId, int milestone);
    }
}