using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;

namespace api.Interfaces.Services
{
    public interface IRefundService
    {
        Task<string> HandleStripeRefund(string orderId, string reason);
        Task<RefundMomoResponseDto> HandleMomoRefund(RefundMomoDto dto);
    }
}