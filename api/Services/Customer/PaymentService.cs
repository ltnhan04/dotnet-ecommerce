using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Repositories;

namespace api.Services.Customer
{
    public class PaymentService : IPaymentRepository
    {
        public readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<VariantPaymentDto> createCheckoutSession(string orderId, List<VariantPaymentDto> variants)
        {
            return await _paymentRepository.createCheckoutSession(orderId, variants);
        }
    }
}