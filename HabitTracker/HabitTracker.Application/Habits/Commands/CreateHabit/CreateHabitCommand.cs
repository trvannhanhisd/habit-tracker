using HabitTracker.Application.Habits.Queries.GetHabits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Habits.Commands.CreateHabit
{
    public class CreateHabitCommand : IRequest<HabitViewModel>
    {
        public int UserId { get; set; }
        public string Title { get; set; } = "";   // Tên thói quen
        public string? Description { get; set; }  // Mô tả
        public string Frequency { get; set; } = "Daily"; // "Daily", "Weekly", etc.
    }
}
