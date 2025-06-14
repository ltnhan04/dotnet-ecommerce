using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace api.Dtos
{
    public class UserDto
    {
        public ObjectId _id { get; set; }
        public required string name { get; set; }
        public required string email { get; set; }
        public string phoneNumber { get; set; } = string.Empty;
        public Address address { get; set; } = new();
        public Role role { get; set; } = Role.user;
        public class Address
        {
            public string street { get; set; } = string.Empty;
            public string ward { get; set; } = string.Empty;
            public string district { get; set; } = string.Empty;
            public string city { get; set; } = string.Empty;
            public string country { get; set; } = string.Empty;
        }

        public enum Role
        {
            user,
            admin
        }

    }
}