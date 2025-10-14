using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using MediatR;


namespace HabitTracker.Application.Features.Habits.Commands.CreateHabit
{
    public class CreateHabitCommand : IRequest<HabitViewModel>
    {

        //[SwaggerSchema(Description = "Tên của thói quen", Example = "Read a Book 📚")] , cách schemea example bằng cách dùng attribute
        public string Title { get; set; } = "";   // Tên thói quen
        public string? Description { get; set; }  // Mô tả
        public string Frequency { get; set; } = "Daily"; // "Daily", "Weekly", etc.
    }
}
