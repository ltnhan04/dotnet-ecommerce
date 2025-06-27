using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.models
{
    public class Product
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [BsonElement("category")]
        public ObjectId category { get; set; }
        [MaxLength(50), BsonElement("name")]
        public required string name { get; set; }
        [BsonElement("description")]
        public required string description { get; set; }
        [BsonElement("variants")]
        public List<ObjectId> variants { get; set; } = new();
        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;

    }

}