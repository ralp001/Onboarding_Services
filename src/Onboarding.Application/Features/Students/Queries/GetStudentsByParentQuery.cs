// Application/Features/Students/Queries/GetStudentsByParent/GetStudentsByParentQuery.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Application.Features.Students.Queries.GetStudentById;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Students.Queries;

public record GetStudentsByParentQuery : IRequest<Result<List<StudentDto>>>
{
    public Guid ParentId { get; init; }
    public Guid UserId { get; init; } // User making the request
    public UserRole UserRole { get; init; }
}