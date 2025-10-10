using HabitTracker.Application.Features.Auth.Commands.Login;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Exceptions.Auth;
using HabitTracker.Domain.Exceptions.User;
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Services;
using HabitTracker.Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HabitTracker.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponseViewModel?>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserContext _userContext;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;
        private readonly IUserRepository _userRepository;

        public RefreshTokenCommandHandler(IAuthRepository authRepository, IUserRepository userRepositoy ,
                                        IConfiguration configuration, IUserContext userContext,
                                        ILogger<RefreshTokenCommandHandler> logger)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _userContext = userContext;
            _logger = logger;
            _userRepository = userRepositoy;
        }

        public async Task<TokenResponseViewModel?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Refreshing token for user ID {UserId}", request.UserId);

            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user == null)
            {
                _logger.LogWarning("Invalid refresh token for user ID {UserId}", request.UserId);
                throw new InvalidRefreshTokenException();
            }

            _logger.LogInformation("Successfully refreshed token for user ID {UserId}", request.UserId);
            return await CreateTokenResponse(user);
        }

        private async Task<User?> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User ID {UserId} not found", userId);
                throw new UserNotFoundException(userId);
            }

            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null; // Will be replaced by throw InvalidRefreshTokenException in Handle
            }

            return user;
        }

        private async Task<TokenResponseViewModel> CreateTokenResponse(User user)
        {
            return new TokenResponseViewModel
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
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
