using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;

namespace api.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Items["user"] is User user && user.role == Role.admin)
            {
                await _next(context);
            }
            else
            {
                await ResponseHandler.SendError(context.Response, "Access denied - Admin only", 403);
            }
        }
    }
}