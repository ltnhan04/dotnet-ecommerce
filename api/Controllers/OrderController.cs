using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Services.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    [Authorize]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        [HttpPost]
        public async Task CreateOrder([FromBody] OrderDto dto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var data = await _orderService.HandleCreateOrder(dto!, userId!);
                await ResponseHandler.SendSuccess(Response, data, 201, "Create order successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}