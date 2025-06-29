using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class PaymentDto
    {
        public string orderId { get; set; } = string.Empty;
        public List<VariantPaymentDto> variants { get; set; } = new();
    }

    public class VariantPaymentDto
    {
        public string variant { get; set; } = string.Empty;
        public int quantity { get; set; }
    }

    public class ProductVariantsPaymentDto
    {
        public string name { get; set; } = string.Empty;
        public int price { get; set; }
        public int quantity { get; set; }
        public string image { get; set; } = string.Empty;
    }

    public class UrlStripe
    {
        public string url { get; set; } = string.Empty;
    }

    public class PaymentMomoDto
    {
        public string orderId { get; set; } = string.Empty;
        public int amount { get; set; }
        public string orderInfo { get; set; } = string.Empty;
    }

    public class UrlMomo
    {
        public string url { get; set; } = string.Empty;
    }

    public class MomoCallbackDto
    {
        public string partnerCode { get; set; } = string.Empty;
        public string orderId { get; set; } = string.Empty;
        public string requestId { get; set; } = string.Empty;
        public long amount { get; set; }
        public string orderInfo { get; set; } = string.Empty;
        public string orderType { get; set; } = string.Empty;
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; } = string.Empty;
        public string payType { get; set; } = string.Empty;
        public string responseTime { get; set; } = string.Empty;
        public string extraData { get; set; } = string.Empty;
        public string signature { get; set; } = string.Empty;
    }

    public class ResponseMomoCallBackDto
    {
        public string success { get; set; } = string.Empty;
        public string orderId { get; set; } = string.Empty;
        public string? transId { get; set; } 
        public string message { get; set; } = string.Empty;
    }
}