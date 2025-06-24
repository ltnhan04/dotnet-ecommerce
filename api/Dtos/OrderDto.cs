using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace api.Dtos
{
    public class OrderDto
    {
        public string _id { get; set; } = string.Empty;
        public List<OrderVariantDetail> variants { get; set; } = new();
        public DateTime createdAt { get; set; }
        public int totalAmount { get; set; }
        public string shippingAddress { get; set; } = string.Empty;
        public string paymentMethod { get; set; } = "pending";

    }
    public class OrderVariantDetail
    {
        public VariantOrderDto variant { get; set; } = new();
        public int quantity { get; set; }

    }
    public class OrderCreateDto
    {
        public string _id { get; set; } = string.Empty;
        public List<OrderCreateVariantDetail> variants { get; set; } = new();
        public DateTime createdAt { get; set; }
        public int totalAmount { get; set; }
        public string status { get; set; } = "pending";
        public string shippingAddress { get; set; } = string.Empty;
        public string paymentMethod { get; set; } = "ship_cod";

    }
    public class OrderCreateVariantDetail
    {
        public string variant { get; set; } = string.Empty;
        public int quantity { get; set; }

    }
    public class VariantOrderDto
    {
        public string _id { get; set; }
        public string product { get; set; } = string.Empty;
        public string productName { get; set; } = string.Empty;
        public string colorName { get; set; } = string.Empty;
        public string colorCode { get; set; } = string.Empty;
        public int stock_quantity { get; set; }
        public string storage { get; set; } = string.Empty;
        public int price { get; set; }
        public List<string> images { get; set; } = new();
    }

    public class Product
    {
        public string name { get; set; } = string.Empty;
    }
    public class Color
    {
        public string colorName { get; set; } = string.Empty;
        public string colorCode { get; set; } = string.Empty;
    }
    public class Customer
    {
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }

    public class OrderDtoResponse
    {
        public string _id { get; set; } = string.Empty;
        public string user { get; set; } =  string.Empty;
        public List<OrderVariantDetail> variants { get; set; } = new();
        public decimal totalAmount { get; set; }
        public string status { get; set; } = string.Empty;
        public string shippingAddress { get; set; } = string.Empty;
        public string paymentMethod { get; set; } = string.Empty;
        public string stripeSessionId { get; set; } = string.Empty;
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class OrderDtoResponseCreate
    {
        public string _id { get; set; }

        public string user { get; set; }
        public List<OrderCreateDto> variants { get; set; } = new();
        public decimal totalAmount { get; set; }
        public string status { get; set; } = string.Empty;
        public string shippingAddress { get; set; } = string.Empty;
        public string paymentMethod { get; set; } = string.Empty;
        public string stripeSessionId { get; set; } = string.Empty;
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class CancelOrderDto
    {
        public string message { get; set; } = string.Empty;
    }
}