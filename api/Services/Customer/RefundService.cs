using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Runtime;
using api.Dtos;
using api.Interfaces;
using api.Interfaces.Services;
using api.Utils;
using MongoDB.Bson.IO;
using Stripe;

namespace api.Services.Customer
{

    public class RefundService : IRefundService
    {
        public readonly IOrderRepository _orderRepository;
        private readonly StripeUtil _stripeUtil;

        public RefundService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _stripeUtil = new StripeUtil();
        }

        public async Task<string> HandleStripeRefund(string orderId, string reason = "requested_by_customer")
        {
            var order = await _orderRepository.GetOrderById(orderId) ?? throw new AppException("Order not found");
            if (order.status != "processing")
            {
                throw new AppException("Only orders in processing status can be refunded.");
            }

            if (order.stripeSessionId == null)
            {
                throw new AppException("This order is not paid");
            }
            var stripeSessionId = order.stripeSessionId;
            if (order.isRefunded == true)
            {
                throw new AppException("This order has already been refunded.");
            }
            var session = await _stripeUtil.GetSessionAsync(stripeSessionId);
            var paymentIntentId = session.PaymentIntentId;
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);

            var refund = await _stripeUtil.CreateRefund(paymentIntentId, paymentIntent.Amount, reason);
            foreach (var item in order.variants)
            {
                var variant = await _orderRepository.GetVariantById(item.variant.ToString());
                if (variant == null)
                {
                    Console.WriteLine("❌ Variant not found: " + item.variant);
                    continue;
                }
                variant.stock_quantity += item.quantity;
                await _orderRepository.UpdateVariant(variant);
            }

            order.isRefunded = true;
            order.refundMethod = "stripe";
            order.refundTransactionId = refund.Id;
            order.refundStatus = refund.Status;
            order.refundAmount = (int?)refund.Amount;
            order.refundReason = reason;
            order.updatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateOrder(order);
            return refund.Id;
        }

        public async Task<RefundMomoResponseDto> HandleMomoRefund(RefundMomoDto dto)
        {
            var order = await _orderRepository.GetOrderById(dto.orderId) ?? throw new AppException("Order not found", 404);

            if (order.status != "processing")
            {
                throw new AppException("Only processing orders can be refunded");
            }

            var payTypeMomo = order.payType.ToLower();
            var refundAmount = order.totalAmount * 0.2;
            var maxRefund = Momo.RefundByPayType(payTypeMomo);

            if (refundAmount < 1000)
            {
                throw new AppException("Refund must be greater than 1000", 400);
            }
            if (refundAmount > maxRefund)
            {
                throw new AppException($"Refund not be exceed by {payTypeMomo}: {maxRefund:N0}đ");
            }

            var partnerCode = Environment.GetEnvironmentVariable("MOMO_PARTNER_CODE");
            var accessKey = Environment.GetEnvironmentVariable("MOMO_ACCESS_KEY");
            var secretKey = Environment.GetEnvironmentVariable("MOMO_SECRET_KEY");

            var refundOrderId = Guid.NewGuid().ToString();
            var requestId = Guid.NewGuid().ToString();
            var description = $"Refund 20% for order {dto.orderId}";
            var transId = order.transId;
            Console.WriteLine(transId);
            var rawData = $"accessKey={accessKey}&amount={refundAmount}&description={description}&orderId={refundOrderId}&partnerCode={partnerCode}&requestId={requestId}&transId={transId}";
            var signature = Momo.CreateSignature(secretKey!, rawData);

            var refundRequest = new RefundMomoRequestDto
            {
                partnerCode = partnerCode!,
                orderId = refundOrderId,
                requestId = requestId,
                amount = (long)refundAmount,
                transId = (long)transId!,
                description = description,
                lang = "vi",
                signature = signature
            };

            var client = new HttpClient();
            var data = new StringContent(JsonSerializer.Serialize(refundRequest), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/refund", data);
            Console.WriteLine(response);
            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var message = json.RootElement.GetProperty("message").GetString() ?? "";
            var responseTime = json.RootElement.GetProperty("responseTime").GetInt64();
            Console.WriteLine($"Refund Request: transId={transId}, refundOrderId={refundOrderId}, amount={refundAmount}");
            Console.WriteLine($"MoMo Refund Raw JSON: {await response.Content.ReadAsStringAsync()}");

            var resultCode = json.RootElement.GetProperty("resultCode").GetInt32();
            if (resultCode != 0)
            {
                var fullJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine(fullJson);
                throw new AppException($"Refund failed {fullJson}", 400);
            }

            order.isRefunded = true;
            order.refundAmount = (int)refundAmount;
            order.refundMethod = "momo";
            order.refundReason = dto.reason;
            order.refundStatus = "succeeded";
            order.refundTransactionId = refundOrderId;
            await _orderRepository.CancelOrder(order._id.ToString());

            return new RefundMomoResponseDto
            {
                partnerCode = partnerCode!,
                orderId = order._id.ToString(),
                requestId = requestId,
                amount = (int)refundAmount,
                transId = (long)transId,
                resultCode = resultCode,
                message = message,
                responseTime = responseTime
            };
        }
    }
}