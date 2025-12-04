// Application/Features/Students/Queries/GetAllStudents/GetAllStudentsQueryHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Application.DTOs;  // Add this
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Students.Queries;

public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, Result<List<StudentDto>>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllStudentsQueryHandler> _logger;

    public GetAllStudentsQueryHandler(
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        ILogger<GetAllStudentsQueryHandler> logger)
    {
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<List<StudentDto>>> Handle(
        GetAllStudentsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate requesting user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<List<StudentDto>>.Failure("User not found");

            // 2. Authorization check - only admins can view all students
            var isAdmin = request.UserRole == UserRole.SuperAdmin ||
                         request.UserRole == UserRole.SchoolAdmin ||
                         request.UserRole == UserRole.AdmissionOfficer;

            if (!isAdmin)
                return Result<List<StudentDto>>.Failure("Unauthorized to view all students");

            // 3. Get all students with optional search
            List<Student> students;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                students = await _studentRepository.SearchAsync(
                    request.SearchTerm,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);
            }
            else
            {
                // Use the GetPagedAsync method
                students = (await _studentRepository.GetPagedAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken)).ToList();
            }

            // 4. Map to DTOs using ACTUAL Student properties
            var studentDtos = students.Select(student => new StudentDto(
                student.Id,
                student.FirstName,
                student.LastName,
                student.MiddleName,
                student.DateOfBirth,
                student.Gender,  // Already string
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
            )).ToList();

            return Result<List<StudentDto>>.Success(studentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all students for user {UserId}", request.UserId);
            return Result<List<StudentDto>>.Failure("An error occurred while fetching students");
        }
    }
}