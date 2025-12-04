// Application/Features/Interviews/Commands/ConductInterview/ConductInterviewCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Interviews.Commands.ConductInterview;

public record ConductInterviewCommand : IRequest<Result>
{
    public Guid InterviewId { get; init; }
    public Guid ConductedByUserId { get; init; } // Interviewer
    public int Score { get; init; } // 0-100
    public string Feedback { get; init; } = string.Empty;
    public string? Remarks { get; init; }
}