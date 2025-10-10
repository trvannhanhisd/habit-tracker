using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogs
{
    public class GetHabitLogsByHabitQuery : IRequest<List<HabitLogViewModel>>
    {
        public int HabitId { get; set; }
    }
}
