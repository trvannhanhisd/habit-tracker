using HabitTracker.Application.Common.ViewModels;
using MediatR;
using static HabitTracker.Domain.Entity.Habit;


namespace HabitTracker.Application.Features.Habits.Commands.CreateHabit
{
    public class CreateHabitCommand : IRequest<HabitViewModel>
    {

        //[SwaggerSchema(Description = "Tên của thói quen", Example = "Read a Book 📚")] , cách schemea example bằng cách dùng attribute
        public string Title { get; set; } = "";   // Tên thói quen
        public string? Description { get; set; }  // Mô tả
        public HabitFrequency Frequency { get; set; }
        public HabitCategory Category { get; set; }
    }
}
