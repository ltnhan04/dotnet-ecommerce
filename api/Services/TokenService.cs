using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.Utils;
using Microsoft.Extensions.Configuration;


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
    }
}