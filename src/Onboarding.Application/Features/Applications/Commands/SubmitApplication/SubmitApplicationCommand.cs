// Application/Features/Applications/Commands/SubmitApplication/SubmitApplicationCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Applications.Commands.SubmitApplication;

public record SubmitApplicationCommand : IRequest<Result<Guid>>
{
    public Guid StudentId { get; init; }
    public Guid ParentUserId { get; init; }
    public EducationalLevel ApplyingForLevel { get; init; }
    public ClassLevel ApplyingForClass { get; init; }
    public SecondaryStream? PreferredStream { get; init; }
}