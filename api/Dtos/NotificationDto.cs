using System;
using MongoDB.Bson;

namespace api.Dtos
{
    public class NotificationDto
    {
        public string _id { get; set; } = string.Empty;
        public string? userId { get; set; }
        public string? targetRole { get; set; }
        public string type { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public bool isRead { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string? redirectUrl { get; set; }
    }
} 