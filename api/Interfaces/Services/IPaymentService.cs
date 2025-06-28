using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;

namespace api.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentDto> HandleCreateCheckoutSession(string orderId, List<VariantPaymentDto> variants);
    }
}