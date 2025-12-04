// Application/Features/Interviews/Commands/ScheduleInterview/ScheduleInterviewCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Interviews.Commands.ScheduleInterview;

public record ScheduleInterviewCommand : IRequest<Result<Guid>>
{
    public Guid ApplicationId { get; init; }
    public Guid ScheduledByUserId { get; init; } // Admin/Admission officer
    public Guid InterviewerId { get; init; } // Staff ID
    public DateTime ScheduledDate { get; init; }
    public TimeSpan ScheduledTime { get; init; }
    public InterviewType Type { get; init; } = InterviewType.InPerson;
    public string? MeetingLink { get; init; }
    public string? MeetingId { get; init; }
}