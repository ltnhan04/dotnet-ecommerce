using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.models
{
    public class Order
    {
        [BsonId]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("user")]
        public ObjectId user { get; set; }

        [BsonElement("variants")]
        public List<OrderVariant> variants { get; set; } = new();

        [BsonElement("totalAmount")]
        public int totalAmount { get; set; }

        [BsonElement("status")]
        public string status { get; set; } = "pending";

        [BsonElement("shippingAddress")]
        public string shippingAddress { get; set; } = string.Empty;

        [BsonElement("paymentMethod")]
        public string paymentMethod { get; set; } = "ship_cod";

        [BsonElement("stripeSessionId")]
        public string? stripeSessionId { get; set; }

        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;

    }

    public class OrderVariant
    {
        [BsonElement("variant")]
        public ObjectId variant { get; set; }

        [BsonElement("quantity")]
        public int quantity { get; set; }
    }

    public enum PaymentMethod
    {
        stripe,

        momo,

        [BsonElement("ship-cod")]
        ship_cod
    }

    public enum OrderStatus
    {
        pending,

        processing,

        shipped,

        delivered,

        cancel
    }
}
