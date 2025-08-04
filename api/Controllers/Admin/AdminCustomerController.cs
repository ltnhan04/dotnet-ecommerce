using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Interfaces.Services;
using api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Services.Customer;

namespace api.Controllers.Admin
{
    [ApiController]
    [Route("v1/admin/customers")]
    [Authorize(Roles = "admin")]
    public class AdminCustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IPointService _pointService;

        public AdminCustomerController(ICustomerService customerService, IPointService pointService)
        {
            _customerService = customerService;
            _pointService = pointService;
        }

        [HttpGet("{customerId}")]
        public async Task GetCustomerById(string customerId)
        {
            try
            {
                var customer = await _customerService.FindCustomerById(customerId);
                await ResponseHandler.SendSuccess(Response, customer, 200, "Get customer successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpGet("{customerId}/points")]
        public async Task GetCustomerPoints(string customerId)
        {
            try
            {
                var points = await _pointService.HandleGetCustomerPoint(customerId);
                await ResponseHandler.SendSuccess(Response, points, 200, "Get customer points successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpGet("{customerId}/vouchers")]
        public async Task GetCustomerVouchers(string customerId)
        {
            try
            {
                var vouchers = await _pointService.HandleGetCustomerVoucher(customerId);
                await ResponseHandler.SendSuccess(Response, vouchers, 200, "Get customer vouchers successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}