using AutoMapper;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Exceptions.Auth;
using HabitTracker.Domain.Repository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserViewModel?>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(IAuthRepository authRepository, IMapper mapper, ILogger<RegisterCommandHandler> logger)
        {
            _authRepository = authRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserViewModel?> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering user {UserName} with email {Email}", request.UserName, request.Email);

            if (await _authRepository.UserExistsAsync(request.UserName))
            {
                _logger.LogWarning("Username {UserName} already exists", request.UserName);
                throw new UsernameAlreadyExistsException(request.UserName);
            }

            if (await _authRepository.EmailExistsAsync(request.Email))
            {
                _logger.LogWarning("Email {Email} already exists", request.Email);
                throw new EmailAlreadyExistsException(request.Email);
            }

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = new PasswordHasher<User>().HashPassword(null!, request.Password)
            };

            var registeredUser = await _authRepository.RegisterUserAsync(user);
            if(registeredUser == null)
            {
                _logger.LogError("Failed to register user {UserName}", request.UserName);
                throw new Exception("User registration failed due to an internal error.");
            }

            _logger.LogInformation("Successfully registered user {UserName} with ID {UserId}", registeredUser.UserName, registeredUser.Id);

            return _mapper.Map<UserViewModel>(registeredUser);
        }
    }
}
