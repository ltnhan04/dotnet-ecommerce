using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace api.models
{
    public class ShippingMethod
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement("name")]
        [Required]
        public string name { get; set; } = ShippingTypes.TieuChuan;

        [BsonElement("basePrice")]
        [Required]
        public double basePrice { get; set; }

        [BsonElement("isActive")]
        public bool isActive { get; set; } = true;

        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    }

    public static class ShippingTypes
    {
        public const string TieuChuan = "Tiêu chuẩn";
        public const string Nhanh = "Nhanh";
        public const string HoaToc = "Hỏa tốc";

        public static readonly List<string> All = new() { TieuChuan, Nhanh, HoaToc };
    }
}
