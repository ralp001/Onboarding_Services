// Application/Features/Students/Queries/GetAllStudents/GetAllStudentsQuery.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Application.Features.Students.Queries.GetStudentById;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Students.Queries;

public record GetAllStudentsQuery : IRequest<Result<List<StudentDto>>>
{
    public Guid UserId { get; init; }
    public UserRole UserRole { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}