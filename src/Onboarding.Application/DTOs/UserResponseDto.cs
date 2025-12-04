// Application/DTOs/Users/UserResponseDto.cs
using Onboarding.Domain.Enums;

namespace Onboarding.Application.DTOs.Users;

public record UserResponseDto(
    Guid Id,
    string Email,
    string PhoneNumber,
    string FirstName,
    string LastName,
    string? MiddleName,
    string FullName,
    UserRole Role,
    UserStatus Status,
    bool IsEmailVerified,
    bool IsPhoneVerified,
    DateTime CreatedAt);