using HabitTracker.Application.Common.Mappings;
using HabitTracker.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.Auth.Commands.Login
{
    public class UserViewModel : IMapFrom<User>
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
