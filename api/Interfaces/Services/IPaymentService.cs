using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;

namespace api.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<UrlStripe> HandleCreateCheckoutSession(string orderId, List<VariantPaymentDto> variants);
        Task<UrlMomo> HandleCreateMomoPayment(PaymentMomoDto dto);
        Task<ResponseMomoCallBackDto> HandleMomoCallback(MomoCallbackDto dto);
        Task<ResponseStripeCallbackDto> HandleStripeCallback(StripeCallbackDto dto);
    }
}