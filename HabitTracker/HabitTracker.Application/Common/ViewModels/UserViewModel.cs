using HabitTracker.Application.Common.Mappings;
using HabitTracker.Domain.Entity;


namespace HabitTracker.Application.Common.ViewModels
{
    public class UserViewModel : IMapFrom<User>
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
