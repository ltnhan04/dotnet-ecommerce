using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using Stripe.Checkout;

namespace api.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<Session> CreateCheckoutSession(string orderId, List<VariantPaymentDto> variants);
        Task<UrlMomo> CreateMomoPayment(PaymentMomoDto dto);
        Task<ResponseMomoCallBackDto> MomoCallback(MomoCallbackDto dto);
        Task<MomoQueryResponseDto> CheckOrderStatusMomo(CheckOrderStatusMomo dto);
    }
}