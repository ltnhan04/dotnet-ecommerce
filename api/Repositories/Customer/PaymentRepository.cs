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
        private readonly IAdminOrderRepository _adminOrderRepository;
        public PaymentRepository(iTribeDbContext context, IHttpClientFactory httpClientFactory, IRedisRepository redisRepository, IAdminOrderRepository adminOrderRepository)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _redisRepository = redisRepository;
            _adminOrderRepository = adminOrderRepository;
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

            var signature = Momo.CreateSignature(secretKey!, rawSignature);

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
                extraData = voucherCode ?? "",
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

            var extraData = dto.extraData ?? "";
            var rawSignature =
            $"accessKey={accessKey}&amount={dto.amount}&extraData={extraData}" +
            $"&message={dto.message}&orderId={dto.orderId}&orderInfo={dto.orderInfo}" +
            $"&orderType={dto.orderType}&partnerCode={partnerCode}&payType={dto.payType}" +
            $"&requestId={dto.requestId}&responseTime={dto.responseTime}&resultCode={dto.resultCode}" +
            $"&transId={dto.transId}";

            var computerSignature = Momo.CreateSignature(secretKey!, rawSignature);

            var isSuccess = dto.resultCode == 0;
            if (isSuccess)
            {
                var orderList = await _context.Orders.ToListAsync();
                var orderSuccess = orderList.FirstOrDefault(item => item._id == ObjectId.Parse(dto.orderId));
                orderSuccess.isPaymentMomo = true;
                await _context.SaveChangesAsync();
            }

            var orders = await _context.Orders.ToListAsync();
            var order = orders.FirstOrDefault(item => item._id == ObjectId.Parse(dto.orderId));

            var variantIds = order.variants.Select(item => item.variant).ToList();
            var variants = await _context.ProductVariants
                .Where(item => variantIds.Contains(item._id))
                .ToListAsync();

            var productIds = variants.Select(item => item.product).ToList();
            var products = await _context.Products
                .Where(item => productIds.Contains(item._id))
                .ToListAsync();

            var variantDict = variants.ToDictionary(v => v._id, v => v);
            var productDict = products.ToDictionary(p => p._id, p => p);

            if (order.status == "pending")
            {
                await _adminOrderRepository.UpdateOrderStatus(order._id.ToString(), new stateDto { status = "processing" });
            }

            order.transId = dto.transId;
            order.payType = dto.payType;
            await _context.SaveChangesAsync();

            return new ResponseMomoCallBackDto
            {
                _id = dto.orderId.ToString(),
                user = order.user.ToString(),
                variants = order.variants.Select(v => new OrderVariantDetail
                {
                    quantity = v.quantity,
                    variant = new VariantOrderDto
                    {
                        _id = v.variant.ToString(),
                        product = variantDict[v.variant].product.ToString(),
                        productName = productDict[variantDict[v.variant].product].name,
                        colorName = variantDict[v.variant].color.colorName,
                        colorCode = variantDict[v.variant].color.colorCode,
                        storage = variantDict[v.variant].storage,
                        price = variantDict[v.variant].price,
                        images = variantDict[v.variant].images ?? new List<string>(),
                    }
                }).ToList(),
                totalAmount = order.totalAmount,
                paymentMethod = order.paymentMethod,
                stripeSessionId = order.stripeSessionId ?? "",
                status = order.status,
                shippingAddress = order.shippingAddress,
                transId = order.transId,
                payType = order.payType,
                createdAt = order.createdAt,
                updatedAt = order.updatedAt
            };
        }

        public async Task<MomoQueryResponseDto> CheckOrderStatusMomo(CheckOrderStatusMomo dto)
        {
            var orders = await _context.Orders.ToListAsync();
            var order = orders.FirstOrDefault(item => item._id == ObjectId.Parse(dto.orderId)) ?? throw new AppException("Order not found", 404);

            var accessKey = Environment.GetEnvironmentVariable("MOMO_ACCESS_KEY");
            var secretKey = Environment.GetEnvironmentVariable("MOMO_SECRET_KEY");
            var partnerCode = Environment.GetEnvironmentVariable("MOMO_PARTNER_CODE");

            var requestId = Guid.NewGuid().ToString();
            var rawData = $"accessKey={accessKey}&orderId={dto.orderId}&partnerCode={partnerCode}&requestId={requestId}";
            var signature = Momo.CreateSignature(secretKey!, rawData);

            var checkRequest = new ResponseCheckOrderStatusMomoDto
            {
                partnerCode = partnerCode!,
                orderId = order._id.ToString(),
                requestId = requestId,
                lang = "vi",
                signature = signature
            };

            var client = new HttpClient();
            var data = new StringContent(JsonSerializer.Serialize(checkRequest), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/query", data);

            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var message = json.RootElement.GetProperty("message").GetString() ?? "";
            var responseTime = json.RootElement.GetProperty("responseTime").GetInt64();

            var resultCode = json.RootElement.GetProperty("resultCode").GetInt32();
            if (resultCode != 0)
            {
                var fullJson = await response.Content.ReadAsStringAsync();
                throw new AppException($"Refund failed {fullJson}", 400);
            }

            var jsonData = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            var refundTrans = new List<MomoRefundTrans>();
            if (jsonData.RootElement.TryGetProperty("refundTrans", out var refundTransElement))
            {
                foreach (var item in refundTransElement.EnumerateArray())
                {
                    refundTrans.Add(new MomoRefundTrans
                    {
                        refundId = item.GetProperty("refundId").GetInt64(),
                        amount = item.GetProperty("amount").GetInt64(),
                        description = item.GetProperty("description").GetString() ?? "",
                        resultCode = item.GetProperty("resultCode").GetInt32(),
                        responseTime = item.GetProperty("responseTime").GetInt64()
                    });
                }
            }

            return new MomoQueryResponseDto
            {
                partnerCode = jsonData.RootElement.GetProperty("partnerCode").GetString() ?? "",
                requestId = jsonData.RootElement.GetProperty("requestId").GetString() ?? "",
                orderId = jsonData.RootElement.GetProperty("orderId").GetString() ?? "",
                extraData = jsonData.RootElement.GetProperty("extraData").GetString() ?? "",
                amount = jsonData.RootElement.GetProperty("amount").GetInt64(),
                transId = jsonData.RootElement.GetProperty("transId").GetInt64(),
                payType = jsonData.RootElement.GetProperty("payType").GetString() ?? "",
                resultCode = jsonData.RootElement.GetProperty("resultCode").GetInt32(),
                refundTrans = refundTrans,
                message = jsonData.RootElement.GetProperty("message").GetString() ?? "",
                responseTime = jsonData.RootElement.GetProperty("responseTime").GetInt64()
            };

        }

    }
}