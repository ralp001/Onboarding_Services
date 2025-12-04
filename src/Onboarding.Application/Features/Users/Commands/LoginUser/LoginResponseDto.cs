
// Application/Features/Users/Commands/LoginUser/LoginResponseDto.cs
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Users.Commands.LoginUser;

public record LoginResponseDto(
    Guid UserId,
    string Email,
    string FullName,
    string Token,
    DateTime TokenExpiry,
    UserRole Role,
    UserStatus Status);