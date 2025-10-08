
using HabitTracker.Domain.Entity;
using HabitTracker.Infrastructure.Repository;
using MediatR;


namespace HabitTracker.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, int>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User { Id = request.Id, UserName = request.UserName, Email = request.Email, Role = request.Role, CreatedAt = request.CreatedAt, IsActive = request.IsActive};
            return await _userRepository.UpdateUserAsync(user);
        }
    }
}
