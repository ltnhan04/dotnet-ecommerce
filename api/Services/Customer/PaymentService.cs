using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.Utils;

namespace api.Services.Customer
{
    public class PaymentService : IPaymentService
    {
        public readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<UrlStripe> HandleCreateCheckoutSession(string orderId, List<VariantPaymentDto> variants)
        {
            var session = await _paymentRepository.CreateCheckoutSession(orderId, variants);
            return new UrlStripe
            {
                url = session.Url
            };
        }

        public async Task<UrlMomo> HandleCreateMomoPayment(PaymentMomoDto dto)
        {
            var momoUrl = await _paymentRepository.CreateMomoPayment(dto);
            return new UrlMomo
            {
                url = momoUrl.url
            };
        }

        public async Task<MomoQueryResponseDto> HandleCheckOrderStatusMomo(CheckOrderStatusMomo dto)
        {
            return await _paymentRepository.CheckOrderStatusMomo(dto);

        }

        public async Task<ResponseMomoCallBackDto> HandleMomoCallback(MomoCallbackDto dto)
        {
            return await _paymentRepository.MomoCallback(dto);
        }
    }
}