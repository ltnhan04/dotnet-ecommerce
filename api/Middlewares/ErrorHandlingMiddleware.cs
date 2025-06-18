using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.Utils;


namespace api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                await HandleExceptionAsync(context, ex);
            }
        }
        public async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode;
            string message;

            if (ex is AppException appEx)
            {
                statusCode = appEx.StatusCode;
                message = appEx.Message;
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "Internal Server Error";
            }

            Console.WriteLine($"[Error] {ex.Message}\n{ex.StackTrace}");

            context.Response.Clear();
            await ResponseHandler.SendError(context.Response, "Internal Server Error", statusCode);
        }
    }
}