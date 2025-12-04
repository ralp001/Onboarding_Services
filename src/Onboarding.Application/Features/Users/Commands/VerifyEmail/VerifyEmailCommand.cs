// Application/Features/Users/Commands/VerifyEmail/VerifyEmailCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;

namespace Onboarding.Application.Features.Users.Commands.VerifyEmail;

public record VerifyEmailCommand : IRequest<Result>
{
    public string Token { get; init; } = string.Empty;
}