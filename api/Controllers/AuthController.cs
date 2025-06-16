using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Services;
using api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly EmailService _emailService;
        public AuthController(AuthService authService, EmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }
        [HttpPost("signup")]
        public async Task SignUp([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.SignUp(dto);
                await _emailService.SendVerificationEmail(result.email, result.verificationCode, EmailTemplates.VerificationEmail(result.verificationCode));
                await ResponseHandler.SendSuccess(Response, null, 200, "Check your email for the OTP");
            }
            catch (Exception ex)
            {

                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}