using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly ICustomerRepository _customerRepository;
        private readonly ITokenService _tokenService;
        private readonly IRedisRepository _redisRepository;
        private readonly IOtpService _otpService;

        public AuthService(ICustomerRepository customerRepository, ITokenService tokenService, IRedisRepository redisRepository, IOtpService otpService)
        {
            _customerRepository = customerRepository;
            _tokenService = tokenService;
            _redisRepository = redisRepository;
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
                {"password", dto.password!},
                { "verificationCode", verificationCode },
                { "createdAt", createdAt.ToString("o") }

            };
            await _redisRepository.SetAsync($"signup:{dto.email}", JsonSerializer.Serialize(otpData), TimeSpan.FromMinutes(5));
            await _redisRepository.SetAsync($"signup:count:{dto.email}", "1", TimeSpan.FromMinutes(10));
            return new OtpResponseDto { verificationCode = verificationCode, email = dto.email };
        }
        public async Task<(string accessToken, string refreshToken)> LoginWithGoogle(string email, string name)
        {

            if (email == null || name == null)
                throw new AppException("Invalid Google data");

            var user = await _customerRepository.FindByEmail(email);
            Console.WriteLine("User: " + user);
            user ??= await _customerRepository.CreateUser(new User
            {
                name = name,
                email = email,
            });

            var tokens = _tokenService.GenerateToken(user._id.ToString(), user.role);
            var accessToken = tokens.accessToken;
            var refreshToken = tokens.refreshToken;
            await _tokenService.StoreRefreshToken(user._id.ToString(), refreshToken.ToString());

            return (accessToken, refreshToken);
        }
        public async Task<VerifiedSignUpDto> VerifyAccount(VerifyOtpDto dto, HttpResponse res)
        {
            var otpDto = await _otpService.VerifyOtp(dto.otp, dto.email) ?? throw new AppException("Verified Account Failed", 400);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(otpDto.password);
            var customer = await _customerRepository.CreateUser(new models.User
            {
                name = otpDto.name,
                email = otpDto.email,
                password = hashedPassword,
            });
            //create first promotion 
            var token = _tokenService.GenerateToken(customer._id.ToString(), customer.role);
            CookieUtil.SetCookie(res, "refreshToken", token.refreshToken);
            await _tokenService.StoreRefreshToken(customer._id.ToString(), token.refreshToken);

            return new VerifiedSignUpDto { accessToken = token.accessToken, name = customer.name, message = "Verified successfully" };
        }
        public async Task<string> ResendOtp(string email)
        {
            var customer = await _customerRepository.FindByEmail(email);
            if (customer != null)
            {
                throw new AppException("Customer already exists", 400);
            }
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
            var customer = await VerifyRole(dto.email, dto.role ?? "user");
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
            var token = _tokenService.GenerateToken(customer._id.ToString(), customer.role);
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

            var decodedToken = _tokenService.ValidateRefreshToken(refreshToken);
            var stored = await _redisRepository.GetAsync($"refresh_token:{decodedToken.userId}");
            if (stored != refreshToken) throw new AppException("Invalid token");

            var tokens = _tokenService.GenerateToken(decodedToken.userId.ToString()!, decodedToken.role);
            CookieUtil.SetCookie(res, "refreshToken", tokens.refreshToken);
            await _redisRepository.SetAsync($"refresh_token:{decodedToken.userId}", tokens.refreshToken, TimeSpan.FromDays(7));
            return new RefreshTokenResponseDto { newAccessToken = tokens.accessToken, message = "Token refreshed" };
        }
        public async Task<UserProfileDto> GetProfile(string userId)
        {
            var customer = await _customerRepository.FindById(userId) ?? throw new AppException("Customer not found");
            UserProfileDto dto = new UserProfileDto
            {
                _id = customer._id.ToString(),
                email = customer.email,
                name = customer.name,
                phoneNumber = customer.phoneNumber!,
                Address = customer.address!,
                role = customer.role.ToString()
            };
            return dto;
        }
        public async Task<UserProfileDto> UpdateProfile(string userId, UpdateCustomerProfileDto dto)
        {
            var customer = await _customerRepository.FindById(userId);
            if (customer == null)
                throw new AppException("User not found", 404);

            customer.name = dto.name;
            customer.phoneNumber = dto.phoneNumber;
            customer.address = new api.models.Address
            {
                street = dto.address.street,
                ward = dto.address.ward,
                district = dto.address.district,
                city = dto.address.city,
                country = dto.address.country
            };

            var result = await _customerRepository.UpdateUser(customer);
            if (result == null)
            {
                throw new AppException("Update profile failed", 400);
            }
            return new UserProfileDto
            {
                _id = result._id.ToString(),
                email = result.email,
                name = result.name,
                phoneNumber = result.phoneNumber!,
                Address = result.address!,
                role = result.role.ToString()
            }; ;
        }

        public async Task<string> ChangePassword(string email)
        {
            var customer = await _customerRepository.FindByEmail(email) ?? throw new AppException("Customer not found");
            var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(20));
            await _redisRepository.SetAsync($"resetpassword:{resetToken}", JsonSerializer.Serialize(new { userId = customer._id.ToString() }), TimeSpan.FromMinutes(5));
            return resetToken;
        }

        public async Task<string> ResetPassword(string token, string newPassword)
        {
            var stored = await _redisRepository.GetAsync($"resetpassword:{token}") ?? throw new AppException("Token expired", 400);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(stored);
            var customer = await _customerRepository.FindById(data["userId"]);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            customer.password = hashedPassword;
            await _customerRepository.UpdateUser(customer);
            return customer.email;
        }
    }
}