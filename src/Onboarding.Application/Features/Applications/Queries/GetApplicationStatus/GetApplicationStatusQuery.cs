// Application/Features/Applications/Queries/GetApplicationStatus/GetApplicationStatusQuery.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Applications.Queries.GetApplicationStatus;

public record GetApplicationStatusQuery : IRequest<Result<ApplicationStatusDto>>
{
    public Guid ApplicationId { get; init; }
    public Guid UserId { get; init; } // Parent or Admin
    public UserRole UserRole { get; init; }
}

