using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace api.models
{
    public class Review
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement("variant")]
        [Required]
        public ObjectId variant { get; set; }

        [BsonElement("user")]
        [Required]
        public ObjectId user { get; set; }

        [BsonElement("rating")]
        [Range(1, 5)]
        public int rating { get; set; }

        [BsonElement("comment")]
        public string? comment { get; set; }

        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    }
}
