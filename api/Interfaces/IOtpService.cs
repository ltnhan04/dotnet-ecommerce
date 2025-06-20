using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;

namespace api.Interfaces
{
    public interface IOtpService
    {
        (string verificationCode, DateTime createdAt) GenerateOtp();
        Task<(OtpDto data, DateTime createdAt)> CheckExpiredOtp(string email);
        Task<OtpDto> VerifyOtp(string otp, string email);
        Task<string> CheckAndResendOtp(string email);
    }
}