using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdOrRole(string userId, string role);
        Task<bool> MarkAsRead(string id);
        Task Create(Notification notification);
    }
}