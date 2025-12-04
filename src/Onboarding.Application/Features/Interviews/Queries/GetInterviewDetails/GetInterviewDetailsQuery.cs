// Application/Features/Interviews/Queries/GetInterviewDetails/GetInterviewDetailsQuery.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Interviews.Queries.GetInterviewDetails;

public record GetInterviewDetailsQuery : IRequest<Result<InterviewDetailsDto>>
{
    public Guid InterviewId { get; init; }
    public Guid UserId { get; init; }
    public UserRole UserRole { get; init; }
}

public record InterviewDetailsDto(
    Guid? InterviewId,
    Guid ApplicationId,
    Guid? StudentId,
    string StudentName,
    DateTime DateOfBirth,
    string Gender,
    string CurrentSchool,
    Guid InterviewerId,
    string InterviewerName,
    DateTime ScheduledDate,
    TimeSpan ScheduledTime,
    InterviewType Type,
    string? MeetingLink,
    string? MeetingId,
    string Status,
    int? Score,
    string? Feedback,
    string? Remarks,
    DateTime? ConductedDate);