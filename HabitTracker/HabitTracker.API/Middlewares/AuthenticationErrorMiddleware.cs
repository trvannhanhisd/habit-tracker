using HabitTracker.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json;

namespace HabitTracker.API.Middlewares
{
    public class AuthenticationErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationErrorMiddleware> _logger;

        public AuthenticationErrorMiddleware(RequestDelegate next, ILogger<AuthenticationErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Gọi middleware tiếp theo trong pipeline
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning(ex, "Token expired for request: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, "Token has expired. Please refresh your token.");
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Invalid token for request: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, "Invalid token.");
            }
            catch (Exception ex) when (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning(ex, "Unauthorized access for request: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, "Authentication required.");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>((int)statusCode, message);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    // Extension để đăng ký middleware
    public static class AuthenticationErrorMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationErrorMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationErrorMiddleware>();
        }
    }
}
