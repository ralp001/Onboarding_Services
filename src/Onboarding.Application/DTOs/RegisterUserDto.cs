// Application/DTOs/Users/RegisterUserDto.cs
using Onboarding.Domain.Enums;

namespace Onboarding.Application.DTOs.Users;

public record RegisterUserDto(
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    UserRole Role = UserRole.Parent,
    string? MiddleName = null,
    string? StateOfOrigin = null,
    string? LocalGovernment = null);