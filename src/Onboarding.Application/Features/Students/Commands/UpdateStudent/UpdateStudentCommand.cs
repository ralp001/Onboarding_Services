// Application/Features/Students/Commands/UpdateStudent/UpdateStudentCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Students.Commands.UpdateStudent;

public record UpdateStudentCommand : IRequest<Result>
{
    public Guid StudentId { get; init; }
    public Guid ParentUserId { get; init; } // Must be the parent

    // Updateable fields
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? Religion { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }

    // Address updates
    public string? AddressStreet { get; init; }
    public string? AddressCity { get; init; }
    public string? AddressLGA { get; init; }
    public NigerianState? AddressState { get; init; }
    public string? AddressPostalCode { get; init; }

    // Academic updates
    public string? PreviousSchool { get; init; }
    public string? PreviousClass { get; init; }
    public string? StateOfOrigin { get; init; }
    public string? LocalGovernment { get; init; }
    public string? Nationality { get; init; }
    public SecondaryStream? SelectedStream { get; init; }
}