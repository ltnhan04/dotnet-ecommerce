using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.models
{
    public class Order
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement("user")]
        public ObjectId user { get; set; }

        [BsonElement("variants")]
        public List<OrderVariant> variants { get; set; } = new();

        [BsonElement("totalAmount")]
        public int totalAmount { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public OrderStatus status { get; set; } = OrderStatus.pending;

        [BsonElement("shippingAddress")]
        public string shippingAddress { get; set; } = string.Empty;

        [BsonElement("paymentMethod")]
        [BsonRepresentation(BsonType.String)]
        public PaymentMethod paymentMethod { get; set; } = PaymentMethod.ship_cod;

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
        [BsonRepresentation(BsonType.String)]
        stripe,

        [BsonRepresentation(BsonType.String)]
        momo,

        [BsonRepresentation(BsonType.String)]
        [BsonElement("ship-cod")]
        ship_cod
    }

    public enum OrderStatus
    {
        [BsonRepresentation(BsonType.String)]
        pending,

        [BsonRepresentation(BsonType.String)]
        processing,

        [BsonRepresentation(BsonType.String)]
        shipped,

        [BsonRepresentation(BsonType.String)]
        delivered,

        [BsonRepresentation(BsonType.String)]
        cancel
    }
}
