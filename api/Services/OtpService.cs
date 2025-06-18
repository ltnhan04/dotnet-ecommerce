using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Utils;

namespace api.Services
{
    public class OtpService : IOtpService
    {
        private readonly IRedisRepository _redis;
        public OtpService(IRedisRepository redis)
        {
            _redis = redis;
        }
        public (string verificationCode, DateTime createdAt) GenerateOtp()
        {
            var verificationCode = new Random().Next(100000, 999999).ToString();
            var createdAt = DateTime.UtcNow;
            return (verificationCode, createdAt);
        }
        public async Task<(OtpDto data, DateTime createdAt)> CheckExpiredOtp(string email)
        {
            var stored = await _redis.GetAsync($"signup:{email}");
            if (string.IsNullOrEmpty(stored))
            {
                throw new AppException("OTP doesn't exist", 404);
            }
            var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(stored);
            if (parsed == null || !parsed.ContainsKey("createdAt"))
                throw new AppException("Invalid OTP data", 500);

            var createdAt = DateTime.Parse(parsed["createdAt"]);
            var timeElapsed = (DateTime.UtcNow - createdAt).TotalSeconds;

            if (timeElapsed > 60)
            {
                throw new AppException("OTP expired", 400);
            }
            return (new OtpDto
            {
                name = parsed["name"],
                password = parsed["password"],
                verificationCode = parsed["verificationCode"],
                email = email,
            }, createdAt);
        }
        public async Task<OtpDto> VerifyOtp(string otp, string email)
        {
            var (otpDto, _) = await CheckExpiredOtp(email);
            var wrongOtpRaw = await _redis.GetAsync($"wrongOtp:{email}");
            int wrongOtp = 0;
            if (!string.IsNullOrEmpty(wrongOtpRaw) && int.TryParse(wrongOtpRaw, out var parsed))
            {
                wrongOtp = parsed;
            }

            if (wrongOtp >= 5)
            {
                throw new AppException("Too many failed attempts, try again in 5 minutes", 429);
            }

            if (string.IsNullOrEmpty(otp))
            {
                throw new AppException("OTP cannot be empty", 400);
            }

            if (string.IsNullOrEmpty(otpDto.verificationCode))
            {
                throw new AppException("Stored verification code is empty", 500);
            }

            if (otp != otpDto.verificationCode)
            {
                wrongOtp++;
                var expiry = wrongOtp < 5 ? TimeSpan.FromMinutes(10) : TimeSpan.FromMinutes(5);
                await _redis.SetAsync($"wrongOtp:{email}", wrongOtp.ToString(), expiry);
                throw new AppException($"Invalid OTP. You have {5 - wrongOtp} attempts remaining.", 400);
            }
            await _redis.DeleteAsync($"wrongOtp:{email}");
            return new OtpDto { name = otpDto.name, password = otpDto.password, email = email };
        }

        public async Task<string> CheckAndResendOtp(string email)
        {
            var stored = await _redis.GetAsync($"signup:{email}");
            if (stored == null)
            {
                throw new AppException("OTP doesn't exist", 404);
            }
            var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(stored);
            if (parsed == null || !parsed.TryGetValue("name", out string? value) || !parsed.TryGetValue("password", out _))
                throw new AppException("Invalid OTP data", 500);
            var name = value;
            var password = value;

            var resendCountRaw = await _redis.GetAsync($"signup:count:{email}");
            int resendCount = 0;
            if (!string.IsNullOrEmpty(resendCountRaw) && int.TryParse(resendCountRaw, out var parsedCount))
            {
                resendCount = parsedCount;
            }

            if (resendCount > 2)
                throw new AppException("You have reached the limit for resending OTP. Please try after 10 mins.", 429);

            var (verificationCode, createdAt) = GenerateOtp();
            var newData = JsonSerializer.Serialize(new Dictionary<string, string>
        {
            {"name", name},
            {"password", password},
            {"verificationCode", verificationCode},
            {"createdAt", createdAt.ToString("o")}
        });

            await _redis.SetAsync($"signup:{email}", newData, TimeSpan.FromMinutes(5));

            if (resendCount == 0)
                await _redis.SetAsync($"signup:count:{email}", "1", TimeSpan.FromMinutes(10));
            else
                await _redis.IncrementAsync($"signup:count:{email}");

            return verificationCode;
        }
    }
}