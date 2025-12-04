// Application/Features/Interviews/Queries/GetUpcomingInterviews/UpcomingInterviewDto.cs
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Interviews.Queries.GetUpcomingInterviews;

public record UpcomingInterviewDto(
    Guid InterviewId,
    Guid ApplicationId,
    string ApplicationNumber,
    string StudentName,
    string ParentName,
    DateTime ScheduledDate,
    TimeSpan ScheduledTime,
    InterviewType Type,
    string? MeetingLink,
    string InterviewerName,
    string Status);