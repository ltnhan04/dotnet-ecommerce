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
        private readonly IConfiguration _config;
        private readonly IRedisRepository _redisRepository;

        public TokenService(IConfiguration configuration, IRedisRepository redisRepository)
        {
            _config = configuration;
            _redisRepository = redisRepository;
        }

        public TokenDto GenerateToken(string userId)
        {
            var accessToken = JwtUtils.GenerateToken(userId, _config["ACCESS_TOKEN_SECRET"]!, 1);
            var refreshToken = JwtUtils.GenerateToken(userId, _config["REFRESH_TOKEN_SECRET"]!, 7);

            return new TokenDto
            {
                accessToken = accessToken,
                refreshToken = refreshToken
            };
        }
        public TokenDto GenerateNewToken(string userId)
        {
            var accessToken = JwtUtils.GenerateToken(userId, _config["ACCESS_TOKEN_SECRET"]!, 1);
            var refreshToken = JwtUtils.GenerateToken(userId, _config["REFRESH_TOKEN_SECRET"]!, 7);
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
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["REFRESH_TOKEN_SECRET"]!);

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

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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