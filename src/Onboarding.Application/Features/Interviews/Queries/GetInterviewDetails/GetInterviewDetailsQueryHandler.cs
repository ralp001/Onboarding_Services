// Application/Features/Interviews/Queries/GetInterviewDetails/GetInterviewDetailsQueryHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Interviews.Queries.GetInterviewDetails;

public class GetInterviewDetailsQueryHandler
    : IRequestHandler<GetInterviewDetailsQuery, Result<InterviewDetailsDto>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetInterviewDetailsQueryHandler> _logger;

    public GetInterviewDetailsQueryHandler(
        IInterviewRepository interviewRepository,
        IApplicationRepository applicationRepository,
        IUserRepository userRepository,
        ILogger<GetInterviewDetailsQueryHandler> logger)
    {
        _interviewRepository = interviewRepository;
        _applicationRepository = applicationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<InterviewDetailsDto>> Handle(
        GetInterviewDetailsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get interview with related data
            var interview = await _interviewRepository.GetByIdAsync(request.InterviewId, cancellationToken);
            if (interview == null)
                return Result<InterviewDetailsDto>.Failure("Interview not found");

            // 2. Validate user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<InterviewDetailsDto>.Failure("User not found");

            // 3. Authorization check
            var isAuthorized = await CheckAuthorization(request.UserId, request.UserRole, interview, cancellationToken);
            if (!isAuthorized)
                return Result<InterviewDetailsDto>.Failure("Unauthorized to view interview details");

            // 4. Map to DTO
            var interviewDto = new InterviewDetailsDto(
                interview.Id,
                interview.ApplicationId,
                interview.StudentId,
                $"{interview.Student?.FirstName} {interview.Student?.LastName}",
                interview.Student?.DateOfBirth ?? DateTime.MinValue,
                interview.Student?.Gender.ToString() ?? "Unknown",
                interview.Student?.PreviousSchool ?? "Not specified",
                interview.InterviewerId ?? Guid.Empty,
                interview.InterviewerName,
                interview.ScheduledDate,
                interview.ScheduledTime,
                interview.Type,
                interview.MeetingLink,
                interview.MeetingId,
                interview.Status.ToString(),
                interview.Score,
                interview.Feedback,
                interview.Remarks,
                interview.ConductedDate
            );

            return Result<InterviewDetailsDto>.Success(interviewDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting interview details {InterviewId}", request.InterviewId);
            return Result<InterviewDetailsDto>.Failure("An error occurred while fetching interview details");
        }
    }

    private async Task<bool> CheckAuthorization(
        Guid userId,
        UserRole userRole,
        Interview interview,
        CancellationToken cancellationToken)
    {
        switch (userRole)
        {
            case UserRole.Parent:
                // Parent can view if it's their child's interview
                var application = await _applicationRepository.GetByIdAsync(interview.ApplicationId, cancellationToken);
                return application?.UserId == userId;

            case UserRole.Teacher:
                // Teacher can view if they are the interviewer
                return interview.InterviewerId == userId;

            case UserRole.SuperAdmin:
            case UserRole.SchoolAdmin:
            case UserRole.AdmissionOfficer:
                // Admins can view all interviews
                return true;

            default:
                return false;
        }
    }
}