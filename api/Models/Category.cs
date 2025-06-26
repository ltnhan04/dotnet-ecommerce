using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.models
{
        public class Category
        {
                [BsonId]
                public ObjectId _id { get; set; }
                [MaxLength(20), BsonElement("name")]
                public required string name { get; set; }
                [BsonElement("parent_category")]
                public ObjectId? parent_category { get; set; }

                [BsonElement("createdAt")]
                public DateTime? createdAt { get; set; }
                [BsonElement("updatedAt")]
                public DateTime? updatedAt { get; set; }
        }
}