using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Services.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    [Authorize]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        [HttpPost]
        public async Task CreateOrder([FromBody] OrderCreateDto dto)
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
        [HttpGet]
        public async Task GetOrderByUser()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var data = await _orderService.HandleGetOrderUser(userId!);
                await ResponseHandler.SendSuccess(Response, data, 200, "Get order successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPut("{orderId}")]
        public async Task CancelOrder(string orderId)
        {
            try
            {
                var data = await _orderService.HandleCancelOrder(orderId);
                await ResponseHandler.SendSuccess(Response, data, 200, "Cancel order successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPost("update-order-payment")]
        public async Task UpdateOrderPayment([FromBody] UpdateOrderPaymentDto dto)
        {
            try
            {
                var data = await _orderService.HandleUpdateOrderPayment(dto);
                await ResponseHandler.SendSuccess(Response, data, 200, data.message);
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}