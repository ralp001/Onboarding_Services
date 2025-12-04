// Application/Features/Users/Commands/RegisterUser/RegisterUserCommandValidator.cs
using FluentValidation;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^(\+234|0)[789][01]\d{8}$").WithMessage("Invalid Nigerian phone number format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid user role")
            .Must(role => role == UserRole.Parent || role == UserRole.Student)
            .WithMessage("Only Parent or Student roles can self-register");

        // Nigerian Context Validation
        RuleFor(x => x.StateOfOrigin)
            .MaximumLength(50).WithMessage("State of origin cannot exceed 50 characters");

        RuleFor(x => x.LocalGovernment)
            .MaximumLength(100).WithMessage("Local government cannot exceed 100 characters");
    }
}