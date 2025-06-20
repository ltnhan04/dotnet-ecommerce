using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class RegisterDto
    {
        public string email { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }
    public class VerifyOtpDto
    {
        public string email { get; set; } = string.Empty;
        public string otp { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public Role role { get; set; }
    }
    public class RefreshTokenDto
    {
        public string refreshToken { get; set; } = string.Empty;
    }
    public class RefreshTokenResponseDto
    {
        public string newAccessToken { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
    }
}

public class GoogleUserDto
{
    public string email { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string picture { get; set; } = string.Empty;
    public string sub { get; set; } = string.Empty;
}
public class CustomerResponseDto
{
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string avatar { get; set; } = string.Empty;
}
public class OtpResponseDto
{
    public string email { get; set; } = string.Empty;
    public string verificationCode { get; set; } = string.Empty;
}
public class LoginGoogleResponseDto
{
    public string accessToken { get; set; } = string.Empty;
    public CustomerResponseDto user { get; set; } = new();

}
public class LoginResponseDto
{
    public string accessToken { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
}
public class VerifiedSignUpDto
{
    public string accessToken { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
}
public class EmailDto
{
    public string email { get; set; } = string.Empty;
}
public class ForgotPasswordDto
{
    public string email { get; set; } = string.Empty;
}
public class ResetPasswordDto
{
    public string password { get; set; } = string.Empty;
}
public class TokenResponseDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("id_token")]
    public string IdToken { get; set; } = string.Empty;
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}
public enum Role
{
    user,
    admin
}
