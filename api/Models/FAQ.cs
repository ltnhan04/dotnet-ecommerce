using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.models
{
    public class FAQ
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement("question")]
        [Required]
        public string question { get; set; } = string.Empty;

        [BsonElement("answer")]
        [Required]
        public string answer { get; set; } = string.Empty;

        [BsonElement("category")]
        public FAQCategory category { get; set; } = FAQCategory.general;

        [BsonElement("keywords")]
        public List<string> keywords { get; set; } = new();

        [BsonElement("usageCount")]
        public int usageCount { get; set; } = 0;

        [BsonElement("lastUsed")]
        public DateTime lastUsed { get; set; } = DateTime.UtcNow;

        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; }
        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; }
    }

    public enum FAQCategory
    {
        order,
        product,
        shipping,
        voucher,
        general
    }
}
