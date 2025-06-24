using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.models
{
    public class ProductVariant
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [BsonElement("product")]
        public ObjectId product { get; set; }
        [BsonElement("rating")]
        public double rating { get; set; } = 0;
        [BsonElement("color")]
        public Color color { get; set; } = new();
        [BsonElement("storage")]
        public required string storage { get; set; }
        [BsonElement("price")]
        public required int price { get; set; }
        [BsonElement("stock_quantity")]
        public required int stock_quantity { get; set; } = 1;
        [BsonElement("slug"), MaxLength(255)]
        public required string slug { get; set; }
        [BsonElement("status")]

        public string status { get; set; } = "in_stock";
        [BsonElement("images")]
        public List<string> images { get; set; } = new();
        [BsonElement("reviews")]
        public List<ObjectId> reviews { get; set; } = new();
        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;

    }
    public class Color
    {

        public string colorName { get; set; } = string.Empty;
        public string colorCode { get; set; } = string.Empty;
    }
}