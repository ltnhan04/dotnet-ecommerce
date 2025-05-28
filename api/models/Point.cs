using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace api.models
{
    public class Point
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement("customer")]
        [Required]
        public ObjectId customer { get; set; }

        [BsonElement("points")]
        [Required]
        public int points { get; set; }

        [BsonElement("order")]
        [Required]
        public ObjectId order { get; set; }

        [BsonElement("expiryDate")]
        [Required]
        public DateTime expiryDate { get; set; }

        [BsonElement("isExpired")]
        public bool isExpired { get; set; } = false;

        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    }
}
