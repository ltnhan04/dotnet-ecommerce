using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;

namespace api.Interfaces.Services
{
    public interface IPointService
    {
        Task<GetCustomerPointDto> HandleGetCustomerPoint(string userId);
        Task<List<GetCustomerVoucherDto>> HandleGetCustomerVoucher(string userId);
        Task<VoucherDto> HandleExchangePointForVoucher(ExchangePointForVoucherDto dto, string userId);
        Task<Point> HandleAddPointForOrder(Order order);
        Task<ApplyVoucherResponseDto> HandleApplyVoucher(ApplyVoucherDto dto, string userId);
        Task<PointVoucherDto> HandleUpdateStatusVoucher(UpdateStatusVoucherDto dto);
        Task<VoucherFreeShipDto> CreateFirstOrderFreeShipPromotion(string customerId);
    }
}