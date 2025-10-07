using HabitTracker.Application.Habits.Queries.GetHabits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Habits.Queries.GetHabitById
{
    public class GetHabitByIdQuery : IRequest<HabitViewModel>
    {
        public int HabitId { get; set; }
    }
}
