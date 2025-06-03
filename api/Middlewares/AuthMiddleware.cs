using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace api.middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        public AuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token == null)
            {
                await ResponseHandler.SendError(context.Response, "You're not authenticated", 401);
                return;
            }
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secret = _configuration["ACCESS_TOKEN_SECRET"];
                if (string.IsNullOrEmpty(secret))
                {
                    await ResponseHandler.SendError(context.Response, "Server configuration error", 500);
                    return;
                }
                var key = Encoding.ASCII.GetBytes(secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "userId").Value;
                var role = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

                var objectId = ObjectId.Parse(userId);
                var roleEnum = Enum.TryParse<Role>(role, true, out var parsedRole) ? parsedRole : Role.user;
                context.Items["User"] = new User { _id = objectId, role = roleEnum };
                await _next(context);
            }
            catch
            {

                await ResponseHandler.SendError(context.Response, "Token is not valid", 403);
            }
        }
    }
}