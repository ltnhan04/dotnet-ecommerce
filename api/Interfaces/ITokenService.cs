using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;

namespace api.Interfaces
{
    public interface ITokenService
    {
        TokenDto GenerateToken(string userId);
        TokenDto GenerateNewToken(string userId);
        Task StoreRefreshToken(string userId, string refreshToken);
    }
}