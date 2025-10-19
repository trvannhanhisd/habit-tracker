using HabitTracker.Domain.Entity;
using MediatR;


namespace HabitTracker.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
