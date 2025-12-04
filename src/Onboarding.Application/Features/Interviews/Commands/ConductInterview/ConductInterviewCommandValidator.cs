// Application/Features/Interviews/Commands/ConductInterview/ConductInterviewCommandValidator.cs
using FluentValidation;

namespace Onboarding.Application.Features.Interviews.Commands.ConductInterview;

public class ConductInterviewCommandValidator : AbstractValidator<ConductInterviewCommand>
{
    public ConductInterviewCommandValidator()
    {
        RuleFor(x => x.InterviewId)
            .NotEmpty().WithMessage("Interview ID is required");

        RuleFor(x => x.ConductedByUserId)
            .NotEmpty().WithMessage("Conducted by user ID is required");

        RuleFor(x => x.Score)
            .InclusiveBetween(0, 100).WithMessage("Score must be between 0 and 100");

        RuleFor(x => x.Feedback)
            .NotEmpty().WithMessage("Feedback is required")
            .MaximumLength(1000).WithMessage("Feedback cannot exceed 1000 characters");

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("Remarks cannot exceed 500 characters");
    }
}