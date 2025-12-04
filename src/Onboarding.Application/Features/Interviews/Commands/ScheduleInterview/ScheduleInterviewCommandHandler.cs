// Application/Features/Interviews/Commands/ScheduleInterview/ScheduleInterviewCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Interviews.Commands.ScheduleInterview;

public class ScheduleInterviewCommandHandler : IRequestHandler<ScheduleInterviewCommand, Result<Guid>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ScheduleInterviewCommandHandler> _logger;

    public ScheduleInterviewCommandHandler(
        IInterviewRepository interviewRepository,
        IApplicationRepository applicationRepository,
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        ILogger<ScheduleInterviewCommandHandler> logger)
    {
        _interviewRepository = interviewRepository;
        _applicationRepository = applicationRepository;
        _staffRepository = staffRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        ScheduleInterviewCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate user scheduling the interview (must be admin/admission officer)
            var scheduler = await _userRepository.GetByIdAsync(request.ScheduledByUserId, cancellationToken);
            if (scheduler == null)
                return Result<Guid>.Failure("Scheduler user not found");

            var isAuthorized = scheduler.Role == UserRole.SuperAdmin ||
                              scheduler.Role == UserRole.SchoolAdmin ||
                              scheduler.Role == UserRole.AdmissionOfficer;

            if (!isAuthorized)
                return Result<Guid>.Failure("Only administrators can schedule interviews");

            // 2. Validate application exists and is approved for interview
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken);
            if (application == null)
                return Result<Guid>.Failure("Application not found");

            if (application.Status != ApplicationStatus.UnderReview)
                return Result<Guid>.Failure("Application must be under review to schedule interview");

            // 3. Validate interviewer exists and is available
            var interviewer = await _staffRepository.GetByIdAsync(request.InterviewerId, cancellationToken);
            if (interviewer == null)
                return Result<Guid>.Failure("Interviewer not found");

            if (interviewer.Status != StaffStatus.Active)
                return Result<Guid>.Failure("Interviewer is not active");

            if (interviewer.Type != StaffType.Teaching)
                return Result<Guid>.Failure("Only teaching staff can conduct interviews");

            // 4. Check for scheduling conflict
            var hasConflict = await _interviewRepository.HasSchedulingConflictAsync(
                request.InterviewerId,
                request.ScheduledDate,
                request.ScheduledTime,
                cancellationToken);

            if (hasConflict)
                return Result<Guid>.Failure("Interviewer already has an interview scheduled at this time");

            // 5. Create interview
            var interview = new Interview
            {
                Id = Guid.NewGuid(),
                ApplicationId = request.ApplicationId,
                StudentId = application.StudentId,
                InterviewerId = request.InterviewerId,
                InterviewerName = $"{interviewer.FirstName} {interviewer.LastName}",
                ScheduledDate = request.ScheduledDate,
                ScheduledTime = request.ScheduledTime,
                Type = request.Type,
                MeetingLink = request.MeetingLink,
                MeetingId = request.MeetingId,
                Status = InterviewStatus.Scheduled,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.ScheduledByUserId
            };

            // 6. Update application status
            application.Status = ApplicationStatus.InterviewScheduled;
            await _applicationRepository.UpdateAsync(application, cancellationToken);

            // 7. Save interview
            await _interviewRepository.AddAsync(interview, cancellationToken);

            _logger.LogInformation(
                "Interview {InterviewId} scheduled for application {ApplicationId} with interviewer {InterviewerId}",
                interview.Id, request.ApplicationId, request.InterviewerId);

            // TODO: Send notification to parent and interviewer

            return Result<Guid>.Success(interview.Id, "Interview scheduled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling interview for application {ApplicationId}", request.ApplicationId);
            return Result<Guid>.Failure("An error occurred while scheduling interview");
        }
    }
}