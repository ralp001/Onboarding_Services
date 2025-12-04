// Application/Features/Students/Queries/GetStudentById/GetStudentByIdQuery.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Students.Queries.GetStudentById;

public record GetStudentByIdQuery : IRequest<Result<StudentDto>>
{
    public Guid StudentId { get; init; }
    public Guid UserId { get; init; }
    public UserRole UserRole { get; init; }
}

public record StudentDto(
    Guid StudentId,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateTime DateOfBirth,
    string Gender,
    string Nationality,
    string StateOfOrigin,
    string LGA,
    string Religion,
    string MedicalConditions,
    string CurrentSchool,
    string CurrentClass,
    string StudentType,
    string Status,
    DateTime CreatedAt);