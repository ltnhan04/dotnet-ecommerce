using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;
using MongoDB.Bson;
using Stripe;

namespace api.Dtos
{
    public class OrderDto
    {
        public string _id { get; set; } = string.Empty;
        public List<OrderVariantDetail> variants { get; set; } = new();
        public string user { get; set; } = string.Empty;
        public DateTime createdAt { get; set; }
        public int totalAmount { get; set; }
        public string shippingAddress { get; set; } = string.Empty;
        public string paymentMethod { get; set; } = "pending";
        public string status { get; set; } = string.Empty;

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
        public string? voucherCode { get; set; } = string.Empty;

    }
    public class OrderCreateVariantDetail
    {
        public string variant { get; set; } = string.Empty;
        public int quantity { get; set; }
    }
    public class VariantOrderDto
    {
        public string _id { get; set; } = string.Empty;
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
        public string user { get; set; } = string.Empty;
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
        public string _id { get; set; } = string.Empty;

        public string user { get; set; } = string.Empty;
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

    public class UpdateOrderPaymentDto
    {
        public string stripeSessionId { get; set; } = string.Empty;
        public string orderId { get; set; } = string.Empty;
    }

    public class UpdateOrderPaymentResponseDto
    {
        public string message { get; set; } = string.Empty;
    }

    public class AdminGetAllOrder
    {
        public string _id { get; set; } = string.Empty;
        public AdminResponseUserDto user { get; set; } = new();
        public List<AdminResponseVariantDto>? variants { get; set; } = new();
        public decimal totalAmount { get; set; }
        public string status { get; set; } = string.Empty;
        public string paymentMethod { get; set; } = string.Empty;
        public DateTime createdAt { get; set; }
        public string stripeSessionId { get; set; } = string.Empty;
        public bool isPaymentMomo { get; set; } = false;
    }

    public class AdminResponseUserDto
    {
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;
        public AdminAddressDto address { get; set; } = new();
    }
    public class AdminAddressDto
    {
        public string? street { get; set; }
        public string? ward { get; set; }
        public string? district { get; set; }
        public string? city { get; set; }
        public string? country { get; set; }
    }

    public class AdminResponseVariantDto
    {
        public AdminResponseColorDto color { get; set; } = new();
        public string storage { get; set; } = string.Empty;
        public string images { get; set; } = string.Empty;
        public int prices { get; set; }
        public int quantity { get; set; }
    }

    public class AdminResponseColorDto
    {
        public string colorName { get; set; } = string.Empty;
        public string colorCode { get; set; } = string.Empty;
    }
    public class AdminResponseUpdateOrderStatus
    {
        public string _id { get; set; } = string.Empty;
        public string user { get; set; } = string.Empty;
        public List<AdminOrderUpdated> variants { get; set; } = new();
        public int totalAmount { get; set; }
        public string status { get; set; } = string.Empty;
        public string shippingAddress { get; set; } = string.Empty;
        public string paymentMethod { get; set; } = string.Empty;
        public string stripeSessionId { get; set; } = string.Empty;

    }

    public class AdminOrderUpdated
    {
        public string variant { get; set; } = string.Empty;
        public int quantity { get; set; }
    }

    public class stateDto
    {
        public string status { get; set; } = string.Empty;
    }

    public class GetOrderQueryDto
    {
        public int page { get; set; } = 1;
        public int size { get; set; } = 10;
        public string? orderId { get; set; }
        public string? customer { get; set; }
        public string? email { get; set; }
        public string? status { get; set; }
        public string? paymentStatus { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}