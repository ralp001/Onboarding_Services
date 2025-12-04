// Application/Features/Interviews/Commands/ScheduleInterview/ScheduleInterviewCommandValidator.cs
using FluentValidation;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Interviews.Commands.ScheduleInterview;

public class ScheduleInterviewCommandValidator : AbstractValidator<ScheduleInterviewCommand>
{
    public ScheduleInterviewCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty().WithMessage("Application ID is required");

        RuleFor(x => x.ScheduledByUserId)
            .NotEmpty().WithMessage("Scheduled by user ID is required");

        RuleFor(x => x.InterviewerId)
            .NotEmpty().WithMessage("Interviewer ID is required");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty().WithMessage("Scheduled date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Scheduled date cannot be in the past");

        RuleFor(x => x.ScheduledTime)
            .NotEmpty().WithMessage("Scheduled time is required");

        // Nigerian school hours: 8 AM - 4 PM
        RuleFor(x => x.ScheduledTime)
            .Must(time => time >= TimeSpan.FromHours(8) && time <= TimeSpan.FromHours(16))
            .WithMessage("Interview must be scheduled between 8 AM and 4 PM");

        // For virtual interviews, meeting link is required
        When(x => x.Type == InterviewType.Virtual, () =>
        {
            RuleFor(x => x.MeetingLink)
                .NotEmpty().WithMessage("Meeting link is required for virtual interviews")
                .Must(link => link.StartsWith("https://"))
                .WithMessage("Meeting link must be a valid HTTPS URL");
        });
    }
}