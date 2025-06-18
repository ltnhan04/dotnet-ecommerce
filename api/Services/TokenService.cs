using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Utils;
using Microsoft.IdentityModel.Tokens;


namespace api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IRedisRepository _redisRepository;

        public TokenService(IRedisRepository redisRepository)
        {
            _redisRepository = redisRepository;

        }

        public TokenDto GenerateToken(string userId)
        {
            var accessTokenSecret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET");
            var refreshTokenSecret = Environment.GetEnvironmentVariable("REFRESH_TOKEN_SECRET");

            if (string.IsNullOrEmpty(accessTokenSecret))
            {
                throw new AppException("ACCESS_TOKEN_SECRET is not configured", 500);
            }

            if (string.IsNullOrEmpty(refreshTokenSecret))
            {
                throw new AppException("REFRESH_TOKEN_SECRET is not configured", 500);
            }

            var accessToken = JwtUtils.GenerateToken(userId, accessTokenSecret, 1);
            var refreshToken = JwtUtils.GenerateToken(userId, refreshTokenSecret, 7);

            return new TokenDto
            {
                accessToken = accessToken,
                refreshToken = refreshToken
            };
        }
        public TokenDto GenerateNewToken(string userId)
        {
            var accessTokenSecret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET");
            var refreshTokenSecret = Environment.GetEnvironmentVariable("REFRESH_TOKEN_SECRET");

            if (string.IsNullOrEmpty(accessTokenSecret))
            {
                throw new AppException("ACCESS_TOKEN_SECRET is not configured", 500);
            }

            if (string.IsNullOrEmpty(refreshTokenSecret))
            {
                throw new AppException("REFRESH_TOKEN_SECRET is not configured", 500);
            }

            var accessToken = JwtUtils.GenerateToken(userId, accessTokenSecret, 1);
            var refreshToken = JwtUtils.GenerateToken(userId, refreshTokenSecret, 7);
            return new TokenDto
            {
                accessToken = accessToken,
                refreshToken = refreshToken
            };
        }
        public async Task StoreRefreshToken(string userId, string refreshToken)
        {
            await _redisRepository.SetAsync($"refresh_token:{userId}", refreshToken, TimeSpan.FromDays(7));
        }
        public string ValidateRefreshToken(string refreshToken)
        {
            var refreshTokenSecret = Environment.GetEnvironmentVariable("REFRESH_TOKEN_SECRET");

            if (string.IsNullOrEmpty(refreshTokenSecret))
            {
                throw new AppException("REFRESH_TOKEN_SECRET is not configured", 500);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(refreshTokenSecret);

            try
            {
                var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var userId = principal.FindFirst("userId")?.Value;
                if (userId == null)
                    throw new AppException("Invalid token", 400);

                return userId;
            }
            catch
            {
                throw new AppException("Invalid refresh token", 400);
            }
        }
    }
}