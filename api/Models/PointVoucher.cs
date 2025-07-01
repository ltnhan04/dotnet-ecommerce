using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace api.models
{
    public class PointVoucher
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement("customer")]
        [Required]
        public ObjectId customer { get; set; }

        [BsonElement("code")]
        [Required]
        public string code { get; set; } = string.Empty;

        [BsonElement("discountAmount")]
        [Required]
        public double discountAmount { get; set; }

        [BsonElement("pointsUsed")]
        [Required]
        public int pointsUsed { get; set; }

        [BsonElement("validFrom")]
        [Required]
        public DateTime validFrom { get; set; }

        [BsonElement("validTo")]
        [Required]
        public DateTime validTo { get; set; }

        [BsonElement("status")]
        public string status { get; set; } = "unused";

        [BsonElement("usedOrder")]
        public ObjectId? usedOrder { get; set; }

        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum VoucherStatus
    {
        unused,
        used,
        expired
    }
}
