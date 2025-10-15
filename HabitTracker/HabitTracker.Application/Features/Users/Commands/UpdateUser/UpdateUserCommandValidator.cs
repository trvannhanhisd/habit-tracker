using FluentValidation;

namespace HabitTracker.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {

            RuleFor(v => v.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MaximumLength(200).WithMessage("UserName must not exceed 200 characters.");

            RuleFor(v => v.Email)
                 .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(200).WithMessage("UserName must not exceed 200 characters.");
        }
    }
}
