using FluentValidation;


namespace HabitTracker.Application.Features.Habits.Commands.CreateHabit
{
    public class CreateHabitCommandValidator : AbstractValidator<CreateHabitCommand>
    {
        public CreateHabitCommandValidator()
        {

            RuleFor(v => v.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(v => v.Description)
                .MaximumLength(200).WithMessage("Description must not exceed 200 characters.");
        }
    }
}
