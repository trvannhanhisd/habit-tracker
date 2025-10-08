using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.Habits.Commands.ArchiveHabit
{
    public class ArchiveHabitCommand : IRequest<int>
    {
        public int HabitId { get; set; }
    }
}
