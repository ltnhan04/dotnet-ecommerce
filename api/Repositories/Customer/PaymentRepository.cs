using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Interfaces.Repositories;
using api.models;
using api.Utils;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Stripe.Checkout;

namespace api.Repositories.Customer
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly iTribeDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRedisRepository _redisRepository;
        public PaymentRepository(iTribeDbContext context, IHttpClientFactory httpClientFactory, IRedisRepository redisRepository)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _redisRepository = redisRepository;
        }

        public async Task<Session> CreateCheckoutSession(string orderId, List<VariantPaymentDto> variants)
        {
            if (variants == null || variants.Count == 0)
            {
                throw new AppException("Invalid product variants", 400);
            }

            var variant = new List<ProductVariantsPaymentDto>();
            foreach (var item in variants)
            {
                var productVariants = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv => pv._id == ObjectId.Parse(item.variant)) ?? throw new AppException($"Product variants {item.variant} not found", 404);

                var products = await _context.Products.ToListAsync();
                var product = products.FirstOrDefault(p => p._id == ObjectId.Parse(productVariants.product.ToString()));

                variant.Add(new ProductVariantsPaymentDto
                {
                    name = product.name,
                    price = productVariants.price,
                    quantity = item.quantity,
                    image = productVariants.images.FirstOrDefault()! ?? "null"
                });
            }

            var lineItems = variant.Select(v =>
            {
                var priceUSD = ChangeRate.priceInUSD(v.price);
                var unitAmount = (long)Math.Round((double)priceUSD * 100);

                return new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = unitAmount,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = v.name,
                            Images = new List<string> { v.image }
                        }
                    },
                    Quantity = v.quantity
                };
            }).ToList();

            var totalAmount = lineItems.Sum(x => x.PriceData.UnitAmount * x.Quantity);
            var voucherCode = await _redisRepository.GetAsync($"voucher-{orderId}");
            if (totalAmount > ChangeRate.vndLimit)
            {
                throw new AppException("Total amount exceed the limit", 400);
            }

            var domain = Environment.GetEnvironmentVariable("CLIENT_URL");

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                LineItems = lineItems,
                SuccessUrl = $"{domain}/payment/success?session_id={{CHECKOUT_SESSION_ID}}&&orderId={orderId}&&voucherCode={voucherCode}",
                CancelUrl = $"{domain}/payment/cancel"
            };
            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session;
        }

        public async Task<UrlMomo> CreateMomoPayment(PaymentMomoDto dto)
        {   
            Console.WriteLine("Callback Momo received");
            var accessKey = Environment.GetEnvironmentVariable("MOMO_ACCESS_KEY");
            var secretKey = Environment.GetEnvironmentVariable("MOMO_SECRET_KEY");
            var partnerCode = Environment.GetEnvironmentVariable("MOMO_PARTNER_CODE");
            var redirectUrl = Environment.GetEnvironmentVariable("CLIENT_URL") + "/payment/success";
            var callbackUrl = Environment.GetEnvironmentVariable("SERVER_URL") + "/api/v1/payment/momo/callback";
            
            var infoPayment = $"Thanh toán đơn hàng {dto.orderId}";
            var voucherCode = await _redisRepository.GetAsync($"voucher-{dto.orderId}");
            var rawSignature =
                $"accessKey={accessKey}&amount={dto.amount}&extraData={voucherCode}&ipnUrl={callbackUrl}" +
                $"&orderId={dto.orderId}&orderInfo={infoPayment}&partnerCode={partnerCode}" +
                $"&redirectUrl={redirectUrl}&requestId={dto.orderId}&requestType=payWithMethod";

            var signature = Momo.ComputerSHA256(secretKey!, rawSignature);

            var requestBody = new
            {
                partnerCode,
                requestId = dto.orderId,
                dto.amount,
                dto.orderId,
                orderInfo = infoPayment,
                redirectUrl,
                ipnUrl = callbackUrl,
                lang = "vi",
                requestType = "payWithMethod",
                autoCapture = true,
                extraData = voucherCode,
                signature
            };

            var client = _httpClientFactory.CreateClient();
            var httpContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", httpContent);
            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            var resultCode = json.RootElement.GetProperty("resultCode").GetInt32();
            if (resultCode != 0)
            {
                throw new AppException("Error result momo", 400);
            }

            return new UrlMomo
            {
                url = json.RootElement.GetProperty("payUrl").GetString()!
            };
        }

        public async Task<ResponseMomoCallBackDto> MomoCallback(MomoCallbackDto dto)
        {
            var accessKey = Environment.GetEnvironmentVariable("MOMO_ACCESS_KEY");
            var secretKey = Environment.GetEnvironmentVariable("MOMO_SECRET_KEY");
            var partnerCode = Environment.GetEnvironmentVariable("MOMO_PARTNER_CODE");

            var rawSignature =
            $"accessKey={accessKey}&amount={dto.amount}&extraData={dto.extraData}" +
            $"&message={dto.message}&orderId={dto.orderId}&orderInfo={dto.orderInfo}" +
            $"&orderType={dto.orderType}&partnerCode={partnerCode}&payType={dto.payType}" +
            $"&requestId={dto.requestId}&responseTime={dto.responseTime}&resultCode={dto.resultCode}" +
            $"&transId={dto.transId}";

            var computerSignature = Momo.ComputerSHA256(secretKey!, rawSignature);

            if (!computerSignature.Equals(dto.signature, StringComparison.OrdinalIgnoreCase))
            {
                return new ResponseMomoCallBackDto
                {
                    success = "false",
                    orderId = dto.orderId,
                    message = "Invalid signature"
                };
            }

            var isSuccess = dto.resultCode == 0;
            if (isSuccess)
            {
                var orders = await _context.Orders.ToListAsync();
                var order = orders.FirstOrDefault(item => item._id == ObjectId.Parse(dto.orderId));
                order.isPaymentMomo = true;
                await _context.SaveChangesAsync();
            }
            return new ResponseMomoCallBackDto
            {
                success = isSuccess.ToString().ToLower(),
                orderId = dto.orderId,
                transId = dto.transId.ToString(),
                message = dto.message
            };
        }
    }
}