using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.models
{
    public class Chatbot
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement("userId")]
        [Required]
        public ObjectId userId { get; set; }

        [BsonElement("sessionId")]
        [Required]
        public string sessionId { get; set; } = string.Empty;

        [BsonElement("messages")]
        public List<ChatMessage> messages { get; set; } = new();

        [BsonElement("context")]
        public Dictionary<string, object> context { get; set; } = new();

        [BsonElement("status")]
        [Required]
        public ChatbotStatus status { get; set; } = ChatbotStatus.active;

        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; }
    }

    public class ChatMessage
    {
        [BsonElement("role")]
        [Required]
        public string role { get; set; } = string.Empty;

        [BsonElement("content")]
        [Required]
        public string content { get; set; } = string.Empty;

        [BsonElement("timestamp")]
        public DateTime timestamp { get; set; } = DateTime.UtcNow;

        public ChatRole Role
        {
            get => Enum.Parse<ChatRole>(role);
            set => role = value.ToString();
        }
    }

    public enum ChatRole
    {
        user,
        assistant
    }

    public enum ChatbotStatus
    {
        active,
        ended
    }
}
