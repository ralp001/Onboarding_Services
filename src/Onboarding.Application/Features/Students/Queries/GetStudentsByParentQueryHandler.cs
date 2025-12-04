// Application/Features/Students/Queries/GetStudentsByParent/GetStudentsByParentQueryHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Application.Features.Students.Queries.GetStudentById;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Students.Queries;

public class GetStudentsByParentQueryHandler : IRequestHandler<GetStudentsByParentQuery, Result<List<StudentDto>>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetStudentsByParentQueryHandler> _logger;

    public GetStudentsByParentQueryHandler(
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        ILogger<GetStudentsByParentQueryHandler> logger)
    {
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<List<StudentDto>>> Handle(
        GetStudentsByParentQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate requesting user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<List<StudentDto>>.Failure("User not found");

            // 2. Authorization check
            var isAuthorized = await CheckAuthorization(request.UserId, request.UserRole, request.ParentId);
            if (!isAuthorized)
                return Result<List<StudentDto>>.Failure("Unauthorized to view these students");

            // 3. Get students by parent
            var students = await _studentRepository.GetByParentIdAsync(request.ParentId, cancellationToken);

            // 4. Map to DTOs
            var studentDtos = students.Select(student => new StudentDto(
                student.Id,
                student.FirstName,
                student.LastName,
                student.MiddleName,
                student.DateOfBirth,
                student.Gender.ToString(),
                student.Nationality,
                student.StateOfOrigin,
                student.LGA,
                student.Religion.ToString(),
                student.MedicalConditions ?? "None",
                student.CurrentSchool,
                student.CurrentClass,
                student.Type.ToString(),
                student.Status.ToString(),
                student.CreatedAt
            )).ToList();

            return Result<List<StudentDto>>.Success(studentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting students for parent {ParentId}", request.ParentId);
            return Result<List<StudentDto>>.Failure("An error occurred while fetching students");
        }
    }

    private async Task<bool> CheckAuthorization(Guid userId, UserRole userRole, Guid parentId)
    {
        switch (userRole)
        {
            case UserRole.Parent:
                // Parent can only view their own children
                return userId == parentId;

            case UserRole.Teacher:
                // Teachers can view students (for potential class assignments)
                return true;

            case UserRole.SuperAdmin:
            case UserRole.SchoolAdmin:
            case UserRole.AdmissionOfficer:
                // Admins can view all students
                return true;

            default:
                return false;
        }
    }
}