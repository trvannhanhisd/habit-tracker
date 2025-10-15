using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.Habits.Commands.UpdateHabit
{
    public class UpdateHabitCommand : IRequest<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string Frequency { get; set; } = "Daily";
        public bool IsArchived { get; set; }
    }
}
