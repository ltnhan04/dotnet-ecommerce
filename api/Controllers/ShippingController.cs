using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using api.Dtos;
using api.Interfaces.Services;
using api.Services.Admin;

namespace api.Controllers
{
    [Route("api/v1/shipping-methods")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingService _shippingService;
        private readonly ILogger<ShippingController> _logger;

        public ShippingController(IShippingService shippingService, ILogger<ShippingController> logger)
        {
            _shippingService = shippingService;
            _logger = logger;
        }

        [HttpPost("calculate-fee")]
        public async Task CalculateShippingFee([FromBody] ShippingDto request)
        {
            try
            {
                var result = await _shippingService.CalculateShippingFeeAsync(request);
                if (result == null)
                {
                    await ResponseHandler.SendError(Response, "Can not calculate fee with GHN.", 400);
                    return;
                }

                await ResponseHandler.SendSuccess(Response, new { methods = result }, 200, "Calculate Fee Successful!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when calculate fee with GHN");
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }

        }
    }
}