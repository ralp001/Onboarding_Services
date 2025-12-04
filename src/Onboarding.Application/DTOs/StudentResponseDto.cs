// Application/Features/Students/DTOs/StudentResponseDto.cs
using Onboarding.Domain.Enums;

namespace Onboarding.Application.DTOs;

public record StudentResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateTime DateOfBirth,
    string Gender,
    string Email,
    string PhoneNumber,
    string FullName,
    int Age,
    StudentStatus Status,
    DateTime CreatedAt);