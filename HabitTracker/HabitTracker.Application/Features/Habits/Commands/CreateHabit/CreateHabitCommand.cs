using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using MediatR;


namespace HabitTracker.Application.Features.Habits.Commands.CreateHabit
{
    public class CreateHabitCommand : IRequest<HabitViewModel>
    {
        public string Title { get; set; } = "";   // Tên thói quen
        public string? Description { get; set; }  // Mô tả
        public string Frequency { get; set; } = "Daily"; // "Daily", "Weekly", etc.
    }
}
