using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dtos;
using api.Services;
using api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;
        public AuthController(AuthService authService, EmailService emailService, IConfiguration config)
        {
            _authService = authService;
            _emailService = emailService;
            _config = config;
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
        [HttpPost("verify-signup")]
        public async Task VerifySignup([FromBody] VerifyOtpDto dto)
        {

            try
            {
                var customer = await _authService.VerifyAccount(dto);
                var result = await _authService.Login(new LoginDto
                {
                    email = dto.email,
                    password = customer.password,
                }, Response);
                await ResponseHandler.SendSuccess(Response, new LoginResponseDto
                {
                    accessToken = result.accessToken,
                    name = result.name,
                    message = "Verified account successfully"
                });
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [HttpPost("resend-otp")]
        public async Task ResendOtp([FromBody] string email)
        {
            try
            {
                var verificationCode = await _authService.ResendOtp(email);
                await _emailService.SendVerificationEmail(email, verificationCode, EmailTemplates.VerificationEmail(verificationCode));
                await ResponseHandler.SendSuccess(Response, null, 200, "Verification code resent successfully.");
            }
            catch (Exception ex)
            {

                await ResponseHandler.SendError(Response, ex.Message, 500);
            }

        }
        [HttpPost("login")]
        public async Task Login([FromBody] LoginDto dto)
        {
            try
            {
                var response = await _authService.Login(dto, Response);
                await ResponseHandler.SendSuccess(Response, new LoginResponseDto
                {
                    accessToken = response.accessToken,
                    name = response.name,
                    message = response.message,
                });
            }
            catch (Exception ex)
            {

                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPost("logout")]
        public async Task Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                var msg = await _authService.Logout(refreshToken!, Response);
                await ResponseHandler.SendSuccess(Response, null, 200, msg);
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [HttpPost("refresh-token")]
        public async Task RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                var result = await _authService.RefreshToken(refreshToken!, Response);
                await ResponseHandler.SendSuccess(Response, new
                {
                    accessToken = result.newAccessToken
                }, 200, result.message);
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [Authorize]
        [HttpGet("profile")]
        public async Task GetProfile()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var customer = await _authService.GetProfile(userId!);
                await ResponseHandler.SendSuccess(Response, customer, 200, "Get profile successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [Authorize]
        [HttpPut("update-profile")]
        public async Task UpdateProfile([FromBody] UpdateCustomerProfileDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var updatedProfile = await _authService.UpdateProfile(userId!, dto);
                await ResponseHandler.SendSuccess(Response, updatedProfile, 200, "Update profile successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }

        }


        [HttpPost("forgot-password")]
        public async Task ForgotPassword([FromBody] string email)
        {
            try
            {
                var resetToken = await _authService.ChangePassword(email);
                await _emailService.SendPasswordResetRequestEmail(email, $"{_config["CLIENT_URL"]}/reset-password/{resetToken}", EmailTemplates.PasswordResetRequestEmail($"{_config["CLIENT_URL"]}/reset-password/{resetToken}"));
                await ResponseHandler.SendSuccess(Response, null, 200, "Password reset link sent to your email");
            }
            catch (Exception ex)
            {

                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPost("reset-password/{token}")]
        public async Task ResetPassword(string token, [FromBody] string password)
        {
            try
            {
                var email = await _authService.ResetPassword(token, password);
                await _emailService.SendPasswordResetSuccessEmail(email, EmailTemplates.PasswordRestSuccessEmail());
                await ResponseHandler.SendSuccess(Response, null, 200, "Password reset successful");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
    }
}