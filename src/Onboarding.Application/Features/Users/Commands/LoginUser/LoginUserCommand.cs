// Application/Features/Users/Commands/LoginUser/LoginUserCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Application.Features.Users.Commands.LoginUser;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Users.Commands.LoginUser;

public record LoginUserCommand : IRequest<Result<LoginResponseDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}
