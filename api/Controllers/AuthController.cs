using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Services;
using api.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly EmailService _emailService;
        public AuthController(IAuthService authService, EmailService emailService)
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
        [HttpPost("verify-signup")]
        public async Task VerifySignup([FromBody] VerifyOtpDto dto)
        {
            try
            {
                var verifiedAccount = await _authService.VerifyAccount(dto, Response);
                await ResponseHandler.SendSuccess(Response, new LoginResponseDto
                {
                    accessToken = verifiedAccount.accessToken,
                    name = verifiedAccount.name,
                    message = "Verified account successfully"
                });
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }
        [HttpPost("resent-otp")]
        public async Task ResendOtp([FromBody] EmailDto dto)
        {
            try
            {
                var verificationCode = await _authService.ResendOtp(dto.email);
                Console.WriteLine("Verification Code" + verificationCode);
                await _emailService.SendVerificationEmail(dto.email, verificationCode, EmailTemplates.VerificationEmail(verificationCode));
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

        [HttpGet("login-google")]
        public IActionResult GoogleLogin()
        {
            Console.WriteLine("Starting Google OAuth login...");
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/api/v1/auth/login-google/callback",
                Items =
                {
                    { ".xsrf", Guid.NewGuid().ToString() }
                }
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("login-google/callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            try
            {
                Console.WriteLine("Google callback received");

                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Console.WriteLine("Result xxxx:     " + result);
                if (!result.Succeeded)
                {
                    Console.WriteLine($"Authentication failed: {result.Failure?.Message}");
                    return Redirect($"{Environment.GetEnvironmentVariable("CLIENT_URL")}/login?error=auth_failed");
                }

                Console.WriteLine($"Authentication succeeded for user: {result.Principal?.Identity?.Name}");

                var (accessToken, refreshToken) = await _authService.LoginWithGoogle(result.Principal!);

                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = false,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                var name = result.Principal?.FindFirst(ClaimTypes.Name)?.Value;
                var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Redirect($"{Environment.GetEnvironmentVariable("CLIENT_URL")}/?accessToken={accessToken}&name={name}&email={email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Google callback error: {ex.Message}");
                return Redirect($"{Environment.GetEnvironmentVariable("CLIENT_URL")}/login?error=callback_failed");
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
                Console.WriteLine(customer);
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
                var userId = User.FindFirst("userId")?.Value;
                var updatedProfile = await _authService.UpdateProfile(userId!, dto);
                await ResponseHandler.SendSuccess(Response, updatedProfile, 200, "Update profile successfully");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }

        }


        [HttpPost("forgot-password")]
        public async Task ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                var resetToken = await _authService.ChangePassword(dto.email);
                await _emailService.SendPasswordResetRequestEmail(dto.email, $"{Environment.GetEnvironmentVariable("CLIENT_URL")}/reset-password/{resetToken}", EmailTemplates.PasswordResetRequestEmail($"{Environment.GetEnvironmentVariable("CLIENT_URL")}/reset-password/{resetToken}"));
                await ResponseHandler.SendSuccess(Response, null, 200, "Password reset link sent to your email");
            }
            catch (Exception ex)
            {
                await ResponseHandler.SendError(Response, ex.Message, 500);
            }
        }

        [HttpPost("reset-password/{token}")]
        public async Task ResetPassword(string token, [FromBody] ResetPasswordDto dto)
        {
            try
            {
                Console.WriteLine("Token: " + token);
                var email = await _authService.ResetPassword(token, dto.password);
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