using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dtos;
using api.models;

namespace api.Interfaces
{
    public interface IAuthService
    {
        Task<OtpResponseDto> SignUp(RegisterDto dto);
        Task<VerifiedSignUpDto> VerifyAccount(VerifyOtpDto dto, HttpResponse res);
        Task<LoginResponseDto> Login(LoginDto dto, HttpResponse res);
        Task<(string accessToken, string refreshToken)> LoginWithGoogle(ClaimsPrincipal principal);

        Task<RefreshTokenResponseDto> RefreshToken(string refreshToken, HttpResponse res);
        Task<string> Logout(string refreshToken, HttpResponse res);
        Task<string> ChangePassword(string email);
        Task<string> ResetPassword(string token, string newPassword);
        Task<UserProfileDto> GetProfile(string userId);
        Task<UserProfileDto> UpdateProfile(string userId, UpdateCustomerProfileDto dto);
        Task<string> ResendOtp(string email);
        Task<User> VerifyRole(string email, string role);
    }
}