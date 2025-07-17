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
        public int amount { get; set; }
        public string orderInfo { get; set; } = string.Empty;
        public string orderType { get; set; } = string.Empty;
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; } = string.Empty;
        public string payType { get; set; } = string.Empty;
        public long responseTime { get; set; }
        public string? extraData { get; set; }
        public string signature { get; set; } = string.Empty;
    }

    public class CheckOrderStatusMomo
    {
        public string orderId { get; set; } = string.Empty;
    }
    public class ResponseCheckOrderStatusMomoDto
    {
        public string partnerCode { get; set; } = string.Empty;
        public string requestId { get; set; } = string.Empty;
        public string orderId { get; set; } = string.Empty;
        public string lang { get; set; } = "vi";
        public string signature { get; set; } = string.Empty;
    }
    public class MomoRefundTrans
    {
        public long refundId { get; set; }
        public long amount { get; set; }
        public string description { get; set; } = string.Empty;
        public int resultCode { get; set; }
        public long responseTime { get; set; }
    }

    public class MomoQueryResponseDto
    {
        public string partnerCode { get; set; } 
        public string requestId { get; set; }
        public string orderId { get; set; }
        public string extraData { get; set; }
        public long amount { get; set; }
        public long transId { get; set; }
        public string payType { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public long responseTime { get; set; }

        public List<MomoRefundTrans>? refundTrans { get; set; }
    }

    public class ResponseMomoCallBackDto : OrderDtoResponse
    {

    }

    public class RefundDto
    {
        public string orderId { get; set; } = string.Empty;
        public string reason { get; set; } = string.Empty;
    }

    public class RefundMomoDto
    {
        public string orderId { get; set; } = string.Empty;
        public string reason { get; set; } = string.Empty;
    }

    public class RefundMomoRequestDto
    {
        public string partnerCode { get; set; } = string.Empty;
        public string orderId { get; set; } = string.Empty;
        public string requestId { get; set; } = string.Empty;
        public long amount { get; set; }
        public long transId { get; set; }
        public string lang { get; set; } = "vi";
        public string description { get; set; } = string.Empty;
        public string signature { get; set; } = string.Empty;

    }
    public class RefundMomoResponseDto
    {
        public string partnerCode { get; set; } = string.Empty;
        public string orderId { get; set; } = string.Empty;
        public string requestId { get; set; } = string.Empty;
        public long amount { get; set; }
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; } = string.Empty;
        public long responseTime { get; set; }
    }
}