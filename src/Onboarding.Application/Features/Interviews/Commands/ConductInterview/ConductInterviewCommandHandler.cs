// Application/Features/Interviews/Commands/ConductInterview/ConductInterviewCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Interviews.Commands.ConductInterview;

public class ConductInterviewCommandHandler : IRequestHandler<ConductInterviewCommand, Result>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly ILogger<ConductInterviewCommandHandler> _logger;

    public ConductInterviewCommandHandler(
        IInterviewRepository interviewRepository,
        IApplicationRepository applicationRepository,
        ILogger<ConductInterviewCommandHandler> logger)
    {
        _interviewRepository = interviewRepository;
        _applicationRepository = applicationRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(ConductInterviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get interview
            var interview = await _interviewRepository.GetByIdAsync(request.InterviewId, cancellationToken);
            if (interview == null)
                return Result.Failure("Interview not found");

            // 2. Verify interviewer is conducting their own interview
            if (interview.InterviewerId != request.ConductedByUserId)
                return Result.Failure("Only the assigned interviewer can conduct this interview");

            // 3. Verify interview is scheduled and not already conducted
            if (interview.Status != InterviewStatus.Scheduled)
                return Result.Failure($"Interview is already {interview.Status.ToString().ToLower()}");

            // 4. Verify interview date/time has arrived
            var interviewDateTime = interview.ScheduledDate.Add(interview.ScheduledTime);
            if (DateTime.UtcNow < interviewDateTime.AddMinutes(-15)) // Can start 15 minutes early
                return Result.Failure("Interview can only be conducted at or after scheduled time");

            // 5. Update interview with results
            interview.Score = request.Score;
            interview.Feedback = request.Feedback;
            interview.Remarks = request.Remarks;
            interview.Status = InterviewStatus.Completed;
            interview.ConductedDate = DateTime.UtcNow;

            await _interviewRepository.UpdateAsync(interview, cancellationToken);

            // 6. Get associated application
            var application = await _applicationRepository.GetByIdAsync(interview.ApplicationId, cancellationToken);
            if (application == null)
                return Result.Failure("Associated application not found");

            // 7. Update application based on interview score
            if (request.Score >= 70) // Pass mark for Nigerian school interviews
            {
                application.Status = ApplicationStatus.Approved;
                application.DecisionDate = DateTime.UtcNow;
                application.DecisionRemarks = $"Interview passed with score: {request.Score}/100. {request.Remarks}";

                // TODO: Auto-admit student if interview passed
                // var student = application.Student;
                // student.Admit($"ADM-{DateTime.UtcNow.Year}-{new Random().Next(1000, 9999)}");
            }
            else
            {
                application.Status = ApplicationStatus.Rejected;
                application.DecisionDate = DateTime.UtcNow;
                application.DecisionRemarks = $"Interview failed with score: {request.Score}/100. {request.Remarks}";
            }

            await _applicationRepository.UpdateAsync(application, cancellationToken);

            _logger.LogInformation(
                "Interview {InterviewId} conducted with score {Score} by user {UserId}",
                request.InterviewId, request.Score, request.ConductedByUserId);

            // TODO: Send notification to parent about interview result

            return Result.Success("Interview conducted and application updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error conducting interview {InterviewId}", request.InterviewId);
            return Result.Failure("An error occurred while conducting interview");
        }
    }
}