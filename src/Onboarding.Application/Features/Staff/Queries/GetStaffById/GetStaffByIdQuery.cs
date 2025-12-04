// Application/Features/Staff/Queries/GetStaffById/GetStaffByIdQuery.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Staff.Queries.GetStaffById;

public record GetStaffByIdQuery : IRequest<Result<StaffDto>>
{
    public Guid StaffId { get; init; }
    public Guid UserId { get; init; }
    public UserRole UserRole { get; init; }
}

public record StaffDto(
    Guid StaffId,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    string Gender,
    StaffType Type,
    string? Department,
    string? Qualification,
    DateTime EmploymentDate,
    string Status,
    DateTime CreatedAt);