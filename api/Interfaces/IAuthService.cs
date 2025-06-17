using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;

namespace api.Interfaces
{
    public interface IAuthService
    {
        Task<OtpResponseDto> SignUp(RegisterDto dto);
        Task<User> VerifyAccount(VerifyOtpDto dto);
        Task<LoginResponseDto> Login(LoginDto dto, HttpResponse res);
        Task<LoginGoogleResponseDto> LoginWithGoogle(GoogleUserDto dto);
        // Task<LoginGoogleResponseDto> GoogleCallBack(GoogleUserDto dto);

        Task<RefreshTokenResponseDto> RefreshToken(string refreshToken, HttpResponse res);
        Task<string> Logout(string refreshToken, HttpResponse res);
        Task<string> ChangePassword(string email);
        Task<string> ResetPassword(string token, string newPassword);
        Task<User> GetProfile(string userId);
        Task<User> UpdateProfile(string userId, UpdateCustomerProfileDto dto);
        Task<string> ResendOtp(string email);
        Task<User> VerifyRole(string email, string role);
    }
}