using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.Habits.Queries.GetHabits
{
    public class GetHabitQuery : IRequest<List<HabitViewModel>>
    {

    }
}
