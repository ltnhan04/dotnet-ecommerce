using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.models;
using api.Repositories;
using api.Repositories.Customer;
using api.Utils;

namespace api.Services
{
    public class AuthService : IAuthService
    {
        private readonly CustomerRepository _customerRepository;
        private readonly TokenService _tokenService;
        private readonly RedisRepository _redisRepository;
        private readonly OtpService _otpService;
        private readonly IConfiguration _configuration;

        public AuthService(CustomerRepository customerRepository, TokenService tokenService, RedisRepository redisRepository, OtpService otpService, IConfiguration configuration)
        {
            _customerRepository = customerRepository;
            _tokenService = tokenService;
            _redisRepository = redisRepository;
            _configuration = configuration;
            _otpService = otpService;
        }

        public async Task<OtpResponseDto> SignUp(RegisterDto dto)
        {
            var existedEmail = await _customerRepository.FindByEmail(dto.email);
            if (existedEmail != null) throw new AppException("Email is already registered", 400);
            var (verificationCode, createdAt) = _otpService.GenerateOtp();
            var otpData = new Dictionary<string, string>
            {
                {"name", dto.name},
                {"email", dto.email},
                { "verificationCode", verificationCode },
                { "createdAt", createdAt.ToString("o") }
            };
            await _redisRepository.SetAsync($"signup:{dto.email}", JsonSerializer.Serialize(otpData), TimeSpan.FromMinutes(5));
            await _redisRepository.SetAsync($"signup:count:{dto.email}", "1", TimeSpan.FromMinutes(10));
            return new OtpResponseDto { verificationCode = verificationCode, email = dto.email };
        }
        public async Task<LoginGoogleResponseDto> LoginWithGoogle(GoogleUserDto dto)
        {
            var customer = await _customerRepository.FindByEmail(dto.email);
            if (customer == null)
            {
                customer = new User
                {
                    email = dto.email,
                    name = dto.name,
                };
                await _customerRepository.CreateUser(customer);
            }
            var token = _tokenService.GenerateToken(customer._id.ToString());
            return new LoginGoogleResponseDto
            {
                accessToken = token.accessToken,
                user = new CustomerResponseDto { id = customer._id.ToString(), name = customer.name, email = customer.email, avatar = dto.picture }
            };
        }
        public async Task<User> VerifyAccount(VerifyOtpDto dto)
        {
            var otpDto = await _otpService.VerifyOtp(dto.email, dto.otp);
            var newCustomer = await _customerRepository.CreateUser(new User
            {
                name = otpDto.name,
                email = otpDto.email,
                password = otpDto.password,
            });
            //create first promotion 
            return newCustomer;
        }
        public async Task<string> ResendOtp(string email)
        {
            var customer = await _customerRepository.FindByEmail(email) ?? throw new AppException("Customer not found", 404);
            var verificationCode = await _otpService.CheckAndResendOtp(email);
            return verificationCode;
        }
        public async Task<User> VerifyRole(string email, string role)
        {
            var customer = await _customerRepository.FindByEmail(email) ?? throw new AppException("Customer not found", 404);
            if (customer.role.ToString() != role)
            {
                throw new AppException("Access denied", 400);
            }
            return customer;
        }
        public async Task<LoginResponseDto> Login(LoginDto dto, HttpResponse res)
        {
            var customer = await VerifyRole(dto.email, dto.role.ToString());
            if (!BCrypt.Net.BCrypt.Verify(dto.password, customer.password))
            {
                var value = await _redisRepository.GetAsync($"wrongPassword:{dto.email}");
                int wrong = int.TryParse(value, out var result) ? result : 0;
                if (wrong >= 5)
                {
                    await _customerRepository.UpdateUser(customer);
                    throw new AppException("Too many failed attempts");
                }
                await _redisRepository.SetAsync($"wrongPassword:{dto.email}", (wrong + 1).ToString(), TimeSpan.FromMinutes(5));
                throw new AppException($"Wrong password. {5 - wrong - 1} attempts left");
            }
            var token = _tokenService.GenerateToken(customer._id.ToString());
            CookieUtil.SetCookie(res, "refreshToken", token.refreshToken);
            await _tokenService.StoreRefreshToken(customer._id.ToString(), token.refreshToken);

            return new LoginResponseDto { accessToken = token.accessToken, name = customer.name, message = "Login success" };
        }
        public async Task<string> Logout(string refreshToken, HttpResponse res)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new AppException("No refresh token provided");
            }

            var userId = _tokenService.ValidateRefreshToken(refreshToken);
            await _redisRepository.DeleteAsync($"refresh_token:{userId}");
            res.Cookies.Delete("refreshToken");
            string message = "Logged out successfully";
            return message;
        }
        public async Task<RefreshTokenResponseDto> RefreshToken(string refreshToken, HttpResponse res)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new AppException("No refresh token provided", 404);

            var userId = _tokenService.ValidateRefreshToken(refreshToken);
            var stored = await _redisRepository.GetAsync($"refresh_token:{userId}");
            if (stored != refreshToken) throw new AppException("Invalid token");

            var tokens = _tokenService.GenerateToken(userId);
            CookieUtil.SetCookie(res, "refreshToken", tokens.refreshToken);
            return new RefreshTokenResponseDto { newAccessToken = tokens.accessToken, message = "Token refreshed" };
        }
        public async Task<User> GetProfile(string userId)
        {
            return await _customerRepository.FindById(userId) ?? throw new AppException("Customer not found");
        }
        public async Task<string> ChangePassword(string email)
        {
            var customer = await _customerRepository.FindByEmail(email);
            if (customer == null) throw new AppException("Customer not found");
            var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(20));
            await _redisRepository.SetAsync($"resetpassword:{resetToken}", JsonSerializer.Serialize(new { userId = customer._id }), TimeSpan.FromMinutes(5));
            return resetToken;
        }

        public async Task<string> ResetPassword(string token, string newPassword)
        {
            var stored = await _redisRepository.GetAsync($"resetpassword:{token}");
            if (stored == null) throw new AppException("Token expired", 400);

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(stored);
            var customer = await _customerRepository.FindById(data["userId"]);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            customer.password = hashedPassword;
            await _customerRepository.UpdateUser(customer);
            return customer.email;
        }
    }
}