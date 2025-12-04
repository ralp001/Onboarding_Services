// Application/Features/Students/DTOs/ParentInfoDto.cs
namespace Onboarding.Application.DTOs;

public record ParentInfoDto(
    string FullName,
    string PhoneNumber,
    string Email,
    string Occupation,
    string Relationship);