using FluentValidation;

namespace HabitTracker.Application.Features.Habits.Commands.UpdateHabit
{
    public class UpdateHabitCommandValidator : AbstractValidator<UpdateHabitCommand>
    {
        public UpdateHabitCommandValidator()
        {

            RuleFor(v => v.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(v => v.Description)
                .MaximumLength(200).WithMessage("Description must not exceed 200 characters.");

            RuleFor(v => v.Frequency)
               .NotEmpty().WithMessage("Frequency is required.");

            RuleFor(v => v.Category)
               .NotEmpty().WithMessage("Frequency is required.");
        }
    }
}
