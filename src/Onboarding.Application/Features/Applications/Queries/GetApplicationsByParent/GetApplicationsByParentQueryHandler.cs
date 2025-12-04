// Application/Features/Applications/Queries/GetApplicationsByParent/GetApplicationsByParentQueryHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Applications.Queries.GetApplicationsByParent;

public class GetApplicationsByParentQueryHandler : IRequestHandler<GetApplicationsByParentQuery, Result<List<ApplicationSummaryDto>>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IInterviewRepository _interviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetApplicationsByParentQueryHandler> _logger;

    public GetApplicationsByParentQueryHandler(
        IApplicationRepository applicationRepository,
        IDocumentRepository documentRepository,
        IInterviewRepository interviewRepository,
        IUserRepository userRepository,
        ILogger<GetApplicationsByParentQueryHandler> logger)
    {
        _applicationRepository = applicationRepository;
        _documentRepository = documentRepository;
        _interviewRepository = interviewRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<List<ApplicationSummaryDto>>> Handle(
        GetApplicationsByParentQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate requesting user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<List<ApplicationSummaryDto>>.Failure("User not found");

            // 2. Authorization check
            if (request.UserRole == UserRole.Parent && request.UserId != request.ParentId)
                return Result<List<ApplicationSummaryDto>>.Failure("Unauthorized to view these applications");

            var isAdmin = request.UserRole == UserRole.SuperAdmin ||
                         request.UserRole == UserRole.SchoolAdmin ||
                         request.UserRole == UserRole.AdmissionOfficer;

            if (!isAdmin && request.UserId != request.ParentId)
                return Result<List<ApplicationSummaryDto>>.Failure("Unauthorized to view these applications");

            // 3. Get applications by parent
            var applications = await _applicationRepository.GetByParentIdAsync(
                request.ParentId,
                request.Status,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            // 4. Get additional data for each application
            var applicationDtos = new List<ApplicationSummaryDto>();

            foreach (var application in applications)
            {
                // Check if application has interview
                var interviews = await _interviewRepository.GetByApplicationIdAsync(application.Id, cancellationToken);
                var hasInterview = interviews != null;

                // Check if all documents are uploaded
                var documents = await _documentRepository.GetByApplicationIdAsync(application.Id, cancellationToken);
                var requiredDocumentTypes = new List<DocumentType>
                {
                    DocumentType.BirthCertificate,
                    DocumentType.PreviousSchoolReport,
                    DocumentType.PassportPhotograph
                };

                // FIXED: Changed d.DocumentType to d.Type (matching your entity)
                var hasAllRequiredDocs = requiredDocumentTypes.All(type =>
                    documents.Any(d => d.Type == type));

                // Get AppliedClass - check what property your Application entity has
                var appliedClass = application switch
                {
                    // If your Application entity has ApplyingForClass (enum)
                    var app when app.GetType().GetProperty("ApplyingForClass") != null =>
                        ((ClassLevel)app.GetType().GetProperty("ApplyingForClass")!.GetValue(app)!).ToString(),

                    // If your Application entity has AppliedClass (string)
                    var app when app.GetType().GetProperty("AppliedClass") != null =>
                        (string)app.GetType().GetProperty("AppliedClass")!.GetValue(app)!,

                    // Default
                    _ => "Not Specified"
                };

                var dto = new ApplicationSummaryDto(
                    application.Id,
                    application.ApplicationNumber ?? $"APP-{application.Id.ToString()[..8].ToUpper()}",
                    application.StudentId,
                    $"{application.Student?.FirstName} {application.Student?.LastName}",
                    appliedClass,
                    application.CreatedAt,
                    application.Status.ToString(),
                    hasInterview,
                    hasAllRequiredDocs
                );

                applicationDtos.Add(dto);
            }

            return Result<List<ApplicationSummaryDto>>.Success(applicationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applications for parent {ParentId}", request.ParentId);
            return Result<List<ApplicationSummaryDto>>.Failure("An error occurred while fetching applications");
        }
    }
}