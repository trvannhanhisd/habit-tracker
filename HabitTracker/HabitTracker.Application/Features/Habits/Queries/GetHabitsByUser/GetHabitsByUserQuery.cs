using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.Habits.Queries.GetHabitsByUser
{
    public class GetHabitsByUserQuery : IRequest<List<HabitViewModel>>
    {
        public int UserId { get; set; }
    }
}
