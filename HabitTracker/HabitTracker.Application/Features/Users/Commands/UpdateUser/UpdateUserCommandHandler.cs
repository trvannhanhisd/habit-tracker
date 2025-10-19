using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Exceptions.User;
using HabitTracker.Domain.Repository;
using HabitTracker.Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, int>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(IUserRepository userRepository, ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating user {UserId}", request.Id);

            var existingUser = await _userRepository.GetUserByIdAsync(request.Id);
            if (existingUser == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", request.Id);
                throw new UserNotFoundException($"User with ID {request.Id} not found.");
            }

            if (string.IsNullOrWhiteSpace(request.UserName))
                throw new ArgumentException("UserName cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email cannot be empty.");

            existingUser.UserName = request.UserName;
            existingUser.Email = request.Email;
            existingUser.Role = request.Role;
            existingUser.IsActive = request.IsActive;

            await _userRepository.UpdateUserAsync(existingUser);

            _logger.LogInformation("User {UserId} updated successfully", request.Id);

            return existingUser.Id;
        }
    }
}
