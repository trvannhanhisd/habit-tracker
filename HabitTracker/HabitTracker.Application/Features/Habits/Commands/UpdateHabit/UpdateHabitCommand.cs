using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HabitTracker.Domain.Entity.Habit;

namespace HabitTracker.Application.Features.Habits.Commands.UpdateHabit
{
    public class UpdateHabitCommand : IRequest<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public HabitFrequency Frequency { get; set; }
        public HabitCategory Category { get; set; } 
        public bool IsArchived { get; set; }
    }
}
