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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiResponse<object>(500, "An unexpected error occurred.");

            switch (ex)
            {
                case HabitNotFoundException hnf:
                    _logger.LogWarning(ex, "Habit not found for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = new ApiResponse<object>(404, hnf.Message);
                    break;

                case HabitLogAlreadyExistsException hlae:
                    _logger.LogWarning(ex, "Habit log already exists for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    response = new ApiResponse<object>(409, hlae.Message);
                    break;

                case UnauthorizedHabitAccessException uha:
                    _logger.LogWarning(ex, "Unauthorized habit access for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response = new ApiResponse<object>(403, uha.Message);
                    break;

                case UsernameAlreadyExistsException uae:
                    _logger.LogWarning(ex, "Username already exists for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new ApiResponse<object>(400, uae.Message);
                    break;

                case EmailAlreadyExistsException eae:
                    _logger.LogWarning(ex, "Email already exists for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new ApiResponse<object>(400, eae.Message);
                    break;

                case InvalidCredentialsException ice:
                    _logger.LogWarning(ex, "Invalid credentials for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = new ApiResponse<object>(401, ice.Message);
                    break;

                case InvalidRefreshTokenException irt:
                    _logger.LogWarning(ex, "Invalid refresh token for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = new ApiResponse<object>(401, irt.Message);
                    break;

                case UserNotFoundException unf:
                    _logger.LogWarning(ex, "User not found for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = new ApiResponse<object>(404, unf.Message);
                    break;

                case InvalidTokenException it:
                    _logger.LogWarning(ex, "Invalid Token for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = new ApiResponse<object>(401, it.Message);
                    break;

                case MissingTokenException mt:
                    _logger.LogWarning(ex, "Missing Token for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = new ApiResponse<object>(401, mt.Message);
                    break;

                case ValidationException ve:
                    _logger.LogWarning(ex, "Validation error for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new ApiResponse<object>(400, ve.Message);
                    break;


                default:
                    _logger.LogError(ex, "Unhandled exception for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = new ApiResponse<object>(500, "An unexpected error occurred.");
                    break;
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
