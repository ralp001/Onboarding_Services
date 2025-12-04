// Domain/ValueObjects/ParentInfo.cs
namespace Onboarding.Domain.ValueObjects;

public record ParentInfo(
    string FullName,
    string PhoneNumber,
    string Email,
    string Occupation,
    string Relationship);