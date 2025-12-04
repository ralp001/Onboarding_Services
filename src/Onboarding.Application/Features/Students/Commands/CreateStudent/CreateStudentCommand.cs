// Application/Features/Students/Commands/CreateStudent/CreateStudentCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Students.Commands.CreateStudent;

public record CreateStudentCommand : IRequest<Result<Guid>>
{
    // Parent User ID (who is creating this student record)
    public Guid ParentUserId { get; init; }

    // Student Personal Information
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string Gender { get; init; } = string.Empty;

    // Student Contact Information
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;

    // Nigerian Address
    public string AddressStreet { get; init; } = string.Empty;
    public string AddressCity { get; init; } = string.Empty;
    public string AddressLGA { get; init; } = string.Empty;
    public NigerianState AddressState { get; init; }
    public string AddressPostalCode { get; init; } = string.Empty;

    // Optional Fields
    public string? MiddleName { get; init; }
    public string? Religion { get; init; }
    public string? PreviousSchool { get; init; }
    public string? PreviousClass { get; init; }
    public string? StateOfOrigin { get; init; }
    public string? LocalGovernment { get; init; }
    public string? Nationality { get; init; }

    // For Senior Secondary Students
    public SecondaryStream? PreferredStream { get; init; }
}