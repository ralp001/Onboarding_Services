// Application/Features/Users/Queries/GetUserProfile/GetUserProfileQuery.cs
using MediatR;
using Onboarding.Application.Common.Models;

namespace Onboarding.Application.Features.Users.Queries.GetUserProfile;

public record GetUserProfileQuery : IRequest<Result<UserProfileDto>>
{
    public Guid UserId { get; init; }
}

