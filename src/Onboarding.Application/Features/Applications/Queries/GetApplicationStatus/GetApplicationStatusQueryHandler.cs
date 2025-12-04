// Application/Features/Applications/Queries/GetApplicationStatus/GetApplicationStatusQueryHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Applications.Queries.GetApplicationStatus;

public class GetApplicationStatusQueryHandler : IRequestHandler<GetApplicationStatusQuery, Result<ApplicationStatusDto>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IInterviewRepository _interviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetApplicationStatusQueryHandler> _logger;

    public GetApplicationStatusQueryHandler(
        IApplicationRepository applicationRepository,
        IDocumentRepository documentRepository,
        IInterviewRepository interviewRepository,
        IUserRepository userRepository,
        ILogger<GetApplicationStatusQueryHandler> logger)
    {
        _applicationRepository = applicationRepository;
        _documentRepository = documentRepository;
        _interviewRepository = interviewRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<ApplicationStatusDto>> Handle(
        GetApplicationStatusQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get application with details
            var application = await _applicationRepository.GetByIdWithDetailsAsync(request.ApplicationId, cancellationToken);
            if (application == null)
                return Result<ApplicationStatusDto>.Failure("Application not found");

            // 2. Validate requesting user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<ApplicationStatusDto>.Failure("User not found");

            // 3. Authorization check
            var isAuthorized = await CheckAuthorization(request.UserId, request.UserRole, application, cancellationToken);
            if (!isAuthorized)
                return Result<ApplicationStatusDto>.Failure("Unauthorized to view application status");

            // 4. Get all required data
            var stages = await GetApplicationStages(application, cancellationToken);
            var requiredDocuments = await GetDocumentStatus(application.Id, cancellationToken);
            var interviewDetails = await GetInterviewDetails(application.Id, cancellationToken);

            // 5. Map to DTO (now matches the new constructor)
            var statusDto = new ApplicationStatusDto(
                ApplicationId: application.Id,
                ApplicationNumber: application.ApplicationNumber ?? $"APP-{application.Id.ToString()[..8].ToUpper()}",
                StudentName: $"{application.Student?.FirstName} {application.Student?.LastName}",
                Status: application.Status,
                AppliedDate: application.CreatedAt,
                DecisionDate: application.DecisionDate,
                DecisionRemarks: application.DecisionRemarks,
                Stages: stages,
                RequiredDocuments: requiredDocuments,
                InterviewDetails: interviewDetails
            );

            return Result<ApplicationStatusDto>.Success(statusDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting application status for {ApplicationId}", request.ApplicationId);
            return Result<ApplicationStatusDto>.Failure("An error occurred while fetching application status");
        }
    }

    private async Task<bool> CheckAuthorization(
        Guid userId,
        UserRole userRole,
        Domain.Entities.Application application,
        CancellationToken cancellationToken)
    {
        switch (userRole)
        {
            case UserRole.Parent:
                return application.UserId == userId;

            case UserRole.SuperAdmin:
            case UserRole.SchoolAdmin:
            case UserRole.AdmissionOfficer:
                return true;

            case UserRole.Teacher:
                var interview = await _interviewRepository.GetByApplicationIdAsync(application.Id, cancellationToken);
                return interview != null && interview.InterviewerId == userId;
            default:
                return false;
        }
    }

    private async Task<List<DocumentStatusDto>> GetDocumentStatus(Guid applicationId, CancellationToken cancellationToken)
    {
        var documents = await _documentRepository.GetByApplicationIdAsync(applicationId, cancellationToken);

        return documents.Select(d => new DocumentStatusDto(
            Type: d.Type,
            Name: d.FileName,
            IsUploaded: true,
            IsVerified: d.IsVerified,
            UploadDate: d.CreatedAt
        )).ToList();
    }

    private async Task<InterviewDetailsDto?> GetInterviewDetails(Guid applicationId, CancellationToken cancellationToken)
    {
        var interview = await _interviewRepository.GetByApplicationIdAsync(applicationId, cancellationToken);
        if (interview == null) return null;

        if (interview == null)
            return null;

        return new InterviewDetailsDto(
            ScheduledDate: interview.ScheduledDate,
            ScheduledTime: interview.ScheduledTime,
            Type: interview.Type,
            Status: interview.Status,
            MeetingLink: interview.MeetingLink,
            InterviewerName: interview.InterviewerName
        );
    }

    private async Task<List<ApplicationStageDto>> GetApplicationStages(
        Domain.Entities.Application application,
        CancellationToken cancellationToken)
    {
        var stages = new List<ApplicationStageDto>();

        // Stage 1: Application Submitted
        stages.Add(new ApplicationStageDto(
            StageName: "Application Submitted",
            Status: "Completed",
            CompletedDate: application.CreatedAt,
            Remarks: $"Application #{application.ApplicationNumber} submitted"
        ));

        // Stage 2: Documents Uploaded
        var documents = await _documentRepository.GetByApplicationIdAsync(application.Id, cancellationToken);
        var documentList = documents?.ToList() ?? new List<Document>();
        var documentsComplete = documentList.Any();
        var allDocumentsVerified = documentList.All(d => d.IsVerified) && documentList.Any();

        stages.Add(new ApplicationStageDto(
            StageName: "Documents Uploaded",
            Status: documentsComplete ? "Completed" : "Pending",
            CompletedDate: documentsComplete ? documentList.Max(d => d.CreatedAt) : null,
            Remarks: documentsComplete ?
                $"{documentList.Count} documents uploaded{(allDocumentsVerified ? " and verified" : "")}" :
                "Awaiting document upload"
        ));

        // Stage 3: Under Review
        var underReviewDate = documentsComplete ? documentList.Max(d => d.CreatedAt).AddDays(1) : (DateTime?)null;
        stages.Add(new ApplicationStageDto(
            StageName: "Under Review",
            Status: application.Status >= ApplicationStatus.UnderReview ? "Completed" : "Pending",
            CompletedDate: application.Status >= ApplicationStatus.UnderReview ? underReviewDate : null,
            Remarks: application.Status >= ApplicationStatus.UnderReview ? "Application is being reviewed" : "Awaiting review"
        ));

        // Stage 4: Interview
        //var interviews = await _interviewRepository.GetByApplicationIdAsync(application.Id, cancellationToken);
        //var interviewList = interviews?.ToList() ?? new List<Interview>();
        //var interview = interviewList.FirstOrDefault();

        // This avoids the ParallelEnumerable conflict completely
        var interview = await _interviewRepository.GetByApplicationIdAsync(application.Id, cancellationToken);

        if (interview != null)
        {
            stages.Add(new ApplicationStageDto(
                StageName: "Interview Scheduled",
                Status: "Completed",
                CompletedDate: interview.CreatedAt,
                Remarks: $"Interview scheduled with {interview.InterviewerName}"
            ));

            if (interview.Status == InterviewStatus.Completed)
            {
                stages.Add(new ApplicationStageDto(
                    StageName: "Interview Conducted",
                    Status: "Completed",
                    CompletedDate: interview.ConductedDate,
                    Remarks: $"Score: {interview.Score}/100"
                ));
            }
        }
        else if (application.Status >= ApplicationStatus.InterviewScheduled)
        {
            stages.Add(new ApplicationStageDto(
                StageName: "Interview",
                Status: application.Status == ApplicationStatus.InterviewScheduled ? "In Progress" : "Pending",
                CompletedDate: null,
                Remarks: "Interview will be scheduled"
            ));
        }

        // Stage 5: Decision
        if (application.Status == ApplicationStatus.Approved ||
            application.Status == ApplicationStatus.Rejected)
        {
            stages.Add(new ApplicationStageDto(
                StageName: "Decision Made",
                Status: "Completed",
                CompletedDate: application.DecisionDate,
                Remarks: $"Application {application.Status.ToString().ToLower()}: {application.DecisionRemarks}"
            ));
        }
        else
        {
            stages.Add(new ApplicationStageDto(
                StageName: "Decision",
                Status: "Pending",
                CompletedDate: null,
                Remarks: "Awaiting final decision"
            ));
        }

        return stages;
    }
}