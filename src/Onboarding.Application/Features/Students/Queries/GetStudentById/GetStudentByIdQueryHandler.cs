// Application/Features/Students/Queries/GetStudentById/GetStudentByIdQueryHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Students.Queries.GetStudentById;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetStudentByIdQueryHandler> _logger;

    public GetStudentByIdQueryHandler(
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        ILogger<GetStudentByIdQueryHandler> logger)
    {
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<StudentDto>> Handle(
        GetStudentByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get student
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null)
                return Result<StudentDto>.Failure("Student not found");

            // 2. Validate requesting user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<StudentDto>.Failure("User not found");

            // 3. Authorization check
            var isAuthorized = await CheckAuthorization(request.UserId, request.UserRole, student, cancellationToken);
            if (!isAuthorized)
                return Result<StudentDto>.Failure("Unauthorized to view student details");

            // 4. Map to DTO using ACTUAL Student properties
            var studentDto = new StudentDto(
                student.Id,
                student.FirstName,
                student.LastName,
                student.MiddleName,
                student.DateOfBirth,
                student.Gender,  // Already string, no .ToString() needed
                student.Nationality ?? "Nigerian",
                student.StateOfOrigin,
                student.LocalGovernment,  // Not LGA
                student.Religion,
                "None",  // MedicalConditions doesn't exist in your entity
                student.PreviousSchool,  // Not CurrentSchool
                student.PreviousClass,   // Not CurrentClass
                "Student",  // Student doesn't have Type property
                student.Status.ToString(),
                student.CreatedAt
            );

            return Result<StudentDto>.Success(studentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student {StudentId}", request.StudentId);
            return Result<StudentDto>.Failure("An error occurred while fetching student details");
        }
    }

    private async Task<bool> CheckAuthorization(
        Guid userId,
        UserRole userRole,
        Student student,
        CancellationToken cancellationToken)
    {
        switch (userRole)
        {
            case UserRole.Parent:
                // Parent can only view their own children
                return student.ParentUserId == userId;  // Changed from UserId to ParentUserId

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