using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Interfaces.Services;
using api.Utils;
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
    }
}