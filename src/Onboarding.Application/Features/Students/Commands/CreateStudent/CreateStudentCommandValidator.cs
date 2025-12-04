// Application/Features/Students/Commands/CreateStudent/CreateStudentCommandValidator.cs
using FluentValidation;

namespace Onboarding.Application.Features.Students.Commands.CreateStudent;

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        // Parent Validation
        RuleFor(x => x.ParentUserId)
            .NotEmpty().WithMessage("Parent user ID is required");

        // Student Validation (same as before, but simplified)
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Now.AddYears(-10)).WithMessage("Student must be at least 10 years old")
            .GreaterThan(DateTime.Now.AddYears(-25)).WithMessage("Student cannot be older than 25 years");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(g => g.Equals("Male", StringComparison.OrdinalIgnoreCase) ||
                      g.Equals("Female", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Gender must be either 'Male' or 'Female'");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^(\+234|0)[789][01]\d{8}$").WithMessage("Invalid Nigerian phone number format");

        // Nigerian Address Validation
        RuleFor(x => x.AddressStreet)
            .NotEmpty().WithMessage("Street address is required")
            .MaximumLength(200).WithMessage("Street address cannot exceed 200 characters");

        RuleFor(x => x.AddressLGA)
            .NotEmpty().WithMessage("Local Government Area is required")
            .MaximumLength(100).WithMessage("LGA cannot exceed 100 characters");
    }
}