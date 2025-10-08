using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.Habits.Queries.GetHabitById
{
    public class GetHabitByIdQuery : IRequest<HabitViewModel>
    {
        public int HabitId { get; set; }
    }
}
