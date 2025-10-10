using HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogById
{
    public class GetHabitLogByIdQuery : IRequest<HabitLogViewModel>
    {
        public int HabitLogId { get; set; }
    }
}
