// Application/Features/Applications/Queries/GetApplicationsByParent/GetApplicationsByParentQuery.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Applications.Queries.GetApplicationsByParent;

public record GetApplicationsByParentQuery : IRequest<Result<List<ApplicationSummaryDto>>>
{
    public Guid ParentId { get; init; }
    public Guid UserId { get; init; }
    public UserRole UserRole { get; init; }
    public ApplicationStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record ApplicationSummaryDto(
    Guid ApplicationId,
    string ApplicationNumber,
    Guid StudentId,
    string StudentName,
    string AppliedClass,
    DateTime AppliedDate,
    string Status,
    bool HasInterview,
    bool DocumentsComplete);