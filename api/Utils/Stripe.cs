using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using Stripe;
using Stripe.Checkout;

namespace api.Utils
{
    public class StripeUtil
    {
        private readonly string secretKey;

        public StripeUtil()
        {
            secretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")!;
            StripeConfiguration.ApiKey = secretKey;
        }

        public async Task<Session> GetSessionAsync(string sessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);
            return session;
        }
        public async Task<Refund> CreateRefund(string paymentIntentId, long amount, string reason = "requested_by_customer")
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
                Amount = amount,
                Reason = reason
            };
            var refundService = new RefundService();
            var refund = await refundService.CreateAsync(options);
            return refund;
        }
    }
}