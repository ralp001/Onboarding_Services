// Application/Features/Applications/Commands/SubmitApplication/SubmitApplicationCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Applications.Commands.SubmitApplication;

public class SubmitApplicationCommandHandler : IRequestHandler<SubmitApplicationCommand, Result<Guid>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SubmitApplicationCommandHandler> _logger;

    public SubmitApplicationCommandHandler(
        IApplicationRepository applicationRepository,
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        ILogger<SubmitApplicationCommandHandler> logger)
    {
        _applicationRepository = applicationRepository;
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        SubmitApplicationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate parent user
            var parentUser = await _userRepository.GetByIdAsync(request.ParentUserId, cancellationToken);
            if (parentUser == null || parentUser.Role != UserRole.Parent)
                return Result<Guid>.Failure("Invalid parent user");

            // 2. Validate student exists and belongs to parent
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null)
                return Result<Guid>.Failure("Student not found");

            if (student.ParentUserId != request.ParentUserId)
                return Result<Guid>.Failure("Student does not belong to this parent");

            // 3. Check if student already has pending/active application
            var existingApplications = await _applicationRepository.GetByStudentIdAsync(
                request.StudentId, cancellationToken);

            var hasActiveApplication = existingApplications.Any(a =>
                a.Status == ApplicationStatus.Draft ||
                a.Status == ApplicationStatus.Submitted ||
                a.Status == ApplicationStatus.UnderReview ||
                a.Status == ApplicationStatus.InterviewScheduled);

            if (hasActiveApplication)
                return Result<Guid>.Failure("Student already has an active application");

            // 4. Check age requirements for Nigerian secondary school
            var age = CalculateAge(student.DateOfBirth);
            var (minAge, maxAge) = GetAgeRequirements(request.ApplyingForLevel);

            if (age < minAge)
                return Result<Guid>.Failure($"Student must be at least {minAge} years old for {request.ApplyingForLevel}");

            if (age > maxAge)
                return Result<Guid>.Failure($"Student cannot be older than {maxAge} years for {request.ApplyingForLevel}");

            // 5. Generate application number
            var applicationNumber = GenerateApplicationNumber();

            // 6. Create application
            Domain.Entities.Application applicationEntity = new()
            {
                Id = Guid.NewGuid(),
                ApplicationNumber = applicationNumber,
                StudentId = request.StudentId,
                UserId = request.ParentUserId,
                ApplyingForLevel = request.ApplyingForLevel,
                ApplyingForClass = request.ApplyingForClass,
                PreferredStream = request.PreferredStream,
                Status = ApplicationStatus.Submitted,
                SubmissionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.ParentUserId
            };

            // 7. Save application
            await _applicationRepository.AddAsync(applicationEntity, cancellationToken);

            _logger.LogInformation(
                "Application {ApplicationId} submitted for student {StudentId} by parent {ParentUserId}",
                applicationEntity.Id, request.StudentId, request.ParentUserId);

            return Result<Guid>.Success(applicationEntity.Id, "Application submitted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting application for student {StudentId}", request.StudentId);
            return Result<Guid>.Failure("An error occurred while submitting application");
        }
    }

    private static string GenerateApplicationNumber()
    {
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month.ToString("D2");
        var day = DateTime.UtcNow.Day.ToString("D2");
        var random = new Random().Next(1000, 9999);

        return $"APP-{year}{month}{day}-{random}";
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age))
            age--;

        return age;
    }

    private static (int MinAge, int MaxAge) GetAgeRequirements(EducationalLevel level)
    {
        return level switch
        {
            EducationalLevel.JuniorSecondary => (10, 15),  // JSS1: 10-15 years
            EducationalLevel.SeniorSecondary => (13, 18),  // SSS1: 13-18 years
            _ => (10, 18)
        };
    }
}