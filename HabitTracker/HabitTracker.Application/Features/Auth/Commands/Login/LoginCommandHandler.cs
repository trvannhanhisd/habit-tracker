using AutoMapper;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Exceptions.Auth;
using HabitTracker.Domain.Exceptions.User;
using HabitTracker.Domain.Repository;
using HabitTracker.Infrastructure.Repository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponseViewModel?>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(IAuthRepository authRepository, IUserRepository userRepository, 
                                    IConfiguration configuration, ILogger<LoginCommandHandler> logger)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<TokenResponseViewModel?> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for user {UserName}", request.UserName);

            var user = await _authRepository.LoginUserAsync(request.UserName);
            if (user == null)
            {
                _logger.LogWarning("User {UserName} not found", request.UserName);
                throw new UserNotFoundException(request.UserName);
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Invalid password for user {UserName}", request.UserName);
                throw new InvalidCredentialsException();
            }

            _logger.LogInformation("Successfully logged in user {UserName}", request.UserName);
            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseViewModel> CreateTokenResponse(User user)
        {
            var refreshToken = await GenerateAndSaveRefreshTokenAsync(user);
            return new TokenResponseViewModel
            {
                AccessToken = CreateToken(user),
                RefreshToken = refreshToken
            };
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateUserAsync(user);
            return refreshToken;
        }
    }
}
