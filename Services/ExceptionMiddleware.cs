    using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net;
using System.Text.Json;

namespace EmployeeManagementSystem.Services

{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);  
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception occurred"); 
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "An unexpected error occurred. Please try again later."
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
