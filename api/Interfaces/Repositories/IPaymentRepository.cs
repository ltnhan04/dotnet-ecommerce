using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;

namespace api.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<VariantPaymentDto> createCheckoutSession(string orderId, List<VariantPaymentDto> variants);
    }
}