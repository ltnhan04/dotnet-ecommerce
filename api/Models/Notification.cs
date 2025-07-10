using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models
{
    public class Notification
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement("userId")]
        public ObjectId? userId { get; set; }

        [BsonElement("targetRole")]
        public string? targetRole { get; set; }

        [BsonElement("title")]
        public string title { get; set; } = string.Empty;

        [BsonElement("message")]
        public string? message { get; set; }

        [BsonElement("type")]
        public string type { get; set; } = "system";

        [BsonElement("isRead")]
        public bool isRead { get; set; } = false;

        [BsonElement("redirectUrl")]
        public string? redirectUrl { get; set; }

        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    }
}