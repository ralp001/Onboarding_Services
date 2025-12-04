// Application/Features/Users/Queries/GetUserProfile/UserProfileDto.cs
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Users.Queries.GetUserProfile;

public record UserProfileDto(
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
    string? StateOfOrigin,
    string? LocalGovernment,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    List<StudentProfileDto> Students); // Parent's children

public record StudentProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    string FullName,
    DateTime DateOfBirth,
    int Age,
    StudentStatus Status,
    string? AdmissionNumber);