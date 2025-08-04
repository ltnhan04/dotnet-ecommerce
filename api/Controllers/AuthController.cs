using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using api.Dtos;
using api.Interfaces;
using api.Services;
using api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("v1/auth")]
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
            var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            var redirectUri = Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URI");
            var scope = "openid email profile";
            var state = Guid.NewGuid().ToString();
            Response.Cookies.Append("GoogleOAuthState", state, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                          $"?client_id={clientId}" +
                          $"&redirect_uri={Uri.EscapeDataString(redirectUri!)}" +
                          $"&response_type=code" +
                          $"&scope={Uri.EscapeDataString(scope)}" +
                          $"&state={state}";

            return Redirect(authUrl);
        }
        [HttpGet("login-google/callback")]
        public async Task<IActionResult> GoogleCallback(string code, string state)
        {
            var storedState = Request.Cookies["GoogleOAuthState"];
            if (string.IsNullOrEmpty(state) || state != storedState)
            {
                return Redirect($"{Environment.GetEnvironmentVariable("CLIENT_URL")}/login?error=invalid_state");
            }

            var client = new HttpClient();
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")! },
            { "client_secret", Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")! },
            { "redirect_uri", "https://itribe.id.vn/api/v1/auth/login-google/callback" },
            { "grant_type", "authorization_code" }
        })
            };

            var tokenResponse = await client.SendAsync(tokenRequest);
            if (!tokenResponse.IsSuccessStatusCode)
            {
                return Redirect($"{Environment.GetEnvironmentVariable("CLIENT_URL")}/login?error=token_exchange_failed");
            }

            var tokenData = await tokenResponse.Content.ReadFromJsonAsync<TokenResponseDto>();
            if (tokenData?.IdToken == null)
            {
                return Redirect($"{Environment.GetEnvironmentVariable("CLIENT_URL")}/login?error=no_id_token");
            }

            var userInfo = await GetUserInfo(tokenData.AccessToken);
            var name = userInfo["name"]?.ToString();
            var email = userInfo["email"]?.ToString();
            var (accessToken, refreshToken) = await _authService.LoginWithGoogle(email!, name!);
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            var frontendUrl = Environment.GetEnvironmentVariable("CLIENT_URL") ?? "http://localhost:3000";
            var redirectUrl = $"{frontendUrl}/login?success=true&accessToken={Uri.EscapeDataString(accessToken)}&name={Uri.EscapeDataString(name ?? "")}&email={Uri.EscapeDataString(email ?? "")}";
            return Redirect(redirectUrl);
        }

        private async Task<Dictionary<string, object>> GetUserInfo(string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
            response.EnsureSuccessStatusCode();
            var userInfo = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>() ?? throw new AppException("Failed to deserialize user info from Google");
            return userInfo;
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
                var email = await _authService.ResetPassword(token, dto.password!);
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