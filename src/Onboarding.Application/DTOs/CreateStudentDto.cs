// Application/Features/Students/DTOs/CreateStudentDto.cs
using Onboarding.Domain.Enums;
using Onboarding.Domain.ValueObjects;

namespace Onboarding.Application.DTOs;

public record CreateStudentDto(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    string Email,
    string PhoneNumber,
    NigerianAddressDto Address,
    ParentInfoDto FatherInfo,
    ParentInfoDto MotherInfo,
    string? MiddleName = null,
    string? Religion = null,
    string? PreviousSchool = null,
    string? PreviousClass = null,
    string? StateOfOrigin = null,
    string? LocalGovernment = null,
    string? Nationality = "Nigerian",
    ParentInfoDto? GuardianInfo = null);