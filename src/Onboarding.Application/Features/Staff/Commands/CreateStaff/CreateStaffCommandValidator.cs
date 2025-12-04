// Application/Features/Staff/Commands/CreateStaff/CreateStaffCommandValidator.cs
using FluentValidation;

namespace Onboarding.Application.Features.Staff.Commands.CreateStaff;

public class CreateStaffCommandValidator : AbstractValidator<CreateStaffCommand>
{
    public CreateStaffCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Valid email address is required");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^(\+234|0)[789][01]\d{8}$")
            .WithMessage("Valid Nigerian phone number required (e.g., 08012345678 or +2348012345678)");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Now.AddYears(-18)).WithMessage("Staff must be at least 18 years old")
            .GreaterThan(DateTime.Now.AddYears(-65)).WithMessage("Staff cannot be older than 65 years");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(g => g == "Male" || g == "Female" || g == "Other")
            .WithMessage("Gender must be Male, Female, or Other");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid staff type");

        RuleFor(x => x.EmploymentDate)
            .NotEmpty().WithMessage("Employment date is required")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Employment date cannot be in the future");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty().WithMessage("Created by user ID is required");
    }
}