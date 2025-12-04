// Application/Features/Staff/Commands/CreateStaff/CreateStaffCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Staff.Commands.CreateStaff;

public record CreateStaffCommand : IRequest<Result<Guid>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string Gender { get; init; } = string.Empty;
    public StaffType Type { get; init; } // Teaching, NonTeaching, Admin
    public string? Department { get; init; }
    public string? Qualification { get; init; }
    public DateTime EmploymentDate { get; init; }
    public Guid CreatedByUserId { get; init; } // Admin who created
}