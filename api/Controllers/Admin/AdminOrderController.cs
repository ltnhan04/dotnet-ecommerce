using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/orders")]
    [Authorize(Roles = "admin")]
    public class AdminOrderController : ControllerBase
    {
        private readonly IAdminOrderService? _orderService;

        public AdminOrderController(IAdminOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task GetAllOrder([FromQuery] GetOrderQueryDto dto)
        {
            try
            {
                var data = await _orderService.HandleGetAllOrder(dto);
                await ResponseHandler.SendSuccess(Response, data, 200, "Get all orders successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpGet("{orderId}")]
        public async Task GetOrderDetail(string orderId)
        {
            try
            {
                var data = await _orderService.HandleGetOrderDetail(orderId);
                await ResponseHandler.SendSuccess(Response, data, 200, "Get order detail successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPut("{orderId}")]
        public async Task UpdateOrderStatus(string orderId, [FromBody] stateDto dto)
        {
            try
            {
                var data = await _orderService.HandleUpdateOrderStatus(orderId, dto);
                await ResponseHandler.SendSuccess(Response, data, 200, "Updated order status successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}