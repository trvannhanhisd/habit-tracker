using HabitTracker.API.Models;
using HabitTracker.Domain.Exceptions;
using HabitTracker.Domain.Exceptions.Auth;
using HabitTracker.Domain.Exceptions.Habit;
using HabitTracker.Domain.Exceptions.HabitLog;
using HabitTracker.Domain.Exceptions.User;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace HabitTracker.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
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

        private static readonly Dictionary<Type, (int StatusCode, string DefaultMessage)> ExceptionMap = new()
{
            { typeof(HabitNotFoundException), (404, "Habit not found.") },
            { typeof(HabitLogAlreadyExistsException), (409, "Habit log already exists.") },
            { typeof(UnauthorizedHabitAccessException), (403, "Unauthorized access to habit.") },
            { typeof(UsernameAlreadyExistsException), (400, "Username already exists.") },
            { typeof(EmailAlreadyExistsException), (400, "Email already exists.") },
            { typeof(InvalidCredentialsException), (401, "Invalid username or password.") },
            { typeof(InvalidRefreshTokenException), (401, "Invalid or expired refresh token.") },
            { typeof(UserNotFoundException), (404, "User not found.") },
            { typeof(InvalidTokenException), (401, "Invalid token.") },
            { typeof(MissingTokenException), (401, "Missing token.") },
            { typeof(ValidationException), (400, "Validation error.") }
        };

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiResponse<object>(500, "An unexpected error occurred.");

            _logger.LogWarning(ex, "Exception occurred for request: {Path}", context.Request.Path);

            if (ExceptionMap.TryGetValue(ex.GetType(), out var errorDetails))
            {
                context.Response.StatusCode = errorDetails.StatusCode;
                response = new ApiResponse<object>(errorDetails.StatusCode, ex.Message ?? errorDetails.DefaultMessage);
            }
            else
            {
                _logger.LogError(ex, "Unhandled exception for request: {Path}", context.Request.Path);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
