// Application/Features/Staff/Queries/GetAvailableInterviewers/GetAvailableInterviewersQuery.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Staff.Queries.GetAvailableInterviewers;

public record GetAvailableInterviewersQuery : IRequest<Result<List<InterviewerDto>>>
{
    public Guid UserId { get; init; }
    public UserRole UserRole { get; init; }
    public DateTime? Date { get; init; } // Optional date to check availability
}

public record InterviewerDto(
    Guid StaffId,
    string FullName,
    string? Department,
    string? Qualification,
    bool IsAvailable,
    DateTime? NextAvailableDate);