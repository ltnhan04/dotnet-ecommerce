using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class OrderDto
    {
        public string _id { get; set; } = string.Empty;
        public DateTime createdAt { get; set; }
        public int totalAmount { get; set; }
        public string shippingAddress { get; set; } = string.Empty;

    }
    public class OrderItem
    {
        public int quantity { get; set; } = 0;
        public Variant variant { get; set; } = new();

    }

    public class Variant
    {
        public string storage { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public Product product { get; set; } = new();
        public Color color { get; set; } = new();
        public List<string> images { get; set; } = new();
        public int price { get; set; } = 0;

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
}