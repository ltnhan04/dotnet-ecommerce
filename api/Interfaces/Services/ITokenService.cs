using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;

namespace api.Interfaces
{
    public interface ITokenService
    {
        TokenDto GenerateToken(string userId, string role);
        TokenDto GenerateNewToken(string userId, string role);
        Task StoreRefreshToken(string userId, string refreshToken);
        ResponseRefreshTokenDto ValidateRefreshToken(string refreshToken);
    }
}