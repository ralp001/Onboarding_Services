// Application/Features/Interviews/Queries/GetUpcomingInterviews/GetUpcomingInterviewsQuery.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Application.Features.Interviews.Queries.GetUpcomingInterviews;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Interviews.Queries.GetUpcomingInterviews;

public record GetUpcomingInterviewsQuery : IRequest<Result<List<UpcomingInterviewDto>>>
{
    public Guid UserId { get; init; }
    public UserRole UserRole { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int? Limit { get; init; } = 50;
}

