using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.models
{
    public class User
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [MaxLength(50), BsonElement("name")]
        public required string name { get; set; }
        [MaxLength(100), BsonElement("email")]
        public required string email { get; set; }
        [MinLength(8), BsonElement("password")]
        public required string password { get; set; }
        [BsonElement("phoneNumber")]
        public string phoneNumber { get; set; } = string.Empty;
        [BsonElement("address")]
        public Address address { get; set; } = new();
        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)]

        public Role role { get; set; } = Role.user;

        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    }
    public class Address
    {
        [MaxLength(50)]
        public string street { get; set; } = string.Empty;
        [MaxLength(50)]
        public string ward { get; set; } = string.Empty;
        [MaxLength(50)]
        public string district { get; set; } = string.Empty;
        [MaxLength(50)]
        public string city { get; set; } = string.Empty;
        [MaxLength(30)]
        public string country { get; set; } = string.Empty;
    }

    public enum Role
    {
        user,
        admin
    }
}

