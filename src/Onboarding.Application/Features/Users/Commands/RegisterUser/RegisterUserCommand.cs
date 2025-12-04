// Application/Features/Users/Commands/RegisterUser/RegisterUserCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Users.Commands.RegisterUser;

public record RegisterUserCommand : IRequest<Result<Guid>>
{
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public UserRole Role { get; init; } = UserRole.Parent;
    public string? MiddleName { get; init; }
    public string? StateOfOrigin { get; init; }
    public string? LocalGovernment { get; init; }
}