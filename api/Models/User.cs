using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.models
{
    public class User
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [MaxLength(50), BsonElement("name")]
        public string name { get; set; } = string.Empty;
        [MaxLength(100), BsonElement("email")]
        public string email { get; set; } = string.Empty;
        [MinLength(8), BsonElement("password")]
        public string? password { get; set; } 
        [BsonElement("phoneNumber")]
        public string? phoneNumber { get; set; }
        [BsonElement("address")]
        [BsonIgnoreIfNull]
        public Address? address { get; set; } = new();
        [BsonElement("role")]
        public string role { get; set; } = "user";

        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class Address
    {
        [MaxLength(50)]
        [BsonElement("street")]
        public string? street { get; set; }

        [MaxLength(50)]
        [BsonElement("ward")]
        public string? ward { get; set; }

        [MaxLength(50)]
        [BsonElement("district")]
        public string? district { get; set; }

        [MaxLength(50)]
        [BsonElement("city")]
        public string? city { get; set; } 

        [MaxLength(30)]
        [BsonElement("country")]
        public string? country { get; set; }
    }

}

