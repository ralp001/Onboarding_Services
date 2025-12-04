// Application/Features/Documents/Queries/GetDocumentsByApplication/GetDocumentsByApplicationQueryHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Documents.Queries.GetDocumentsByApplication;

public class GetDocumentsByApplicationQueryHandler
    : IRequestHandler<GetDocumentsByApplicationQuery, Result<List<DocumentDto>>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetDocumentsByApplicationQueryHandler> _logger;

    public GetDocumentsByApplicationQueryHandler(
        IDocumentRepository documentRepository,
        IApplicationRepository applicationRepository,
        IUserRepository userRepository,
        ILogger<GetDocumentsByApplicationQueryHandler> logger)
    {
        _documentRepository = documentRepository;
        _applicationRepository = applicationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<List<DocumentDto>>> Handle(
        GetDocumentsByApplicationQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<List<DocumentDto>>.Failure("User not found");

            // 2. Get application
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken);
            if (application == null)
                return Result<List<DocumentDto>>.Failure("Application not found");

            // 3. Authorization check
            if (request.UserRole == UserRole.Parent && application.UserId != request.UserId)
                return Result<List<DocumentDto>>.Failure("Unauthorized to view these documents");

            var isAdmin = request.UserRole == UserRole.SuperAdmin ||
                         request.UserRole == UserRole.SchoolAdmin ||
                         request.UserRole == UserRole.AdmissionOfficer;

            if (!isAdmin && application.UserId != request.UserId)
                return Result<List<DocumentDto>>.Failure("Unauthorized to view these documents");

            // 4. Get documents
            var documents = await _documentRepository.GetByApplicationIdAsync(request.ApplicationId, cancellationToken);

            // 5. Map to DTOs
            var documentDtos = documents.Select(d => new DocumentDto(
                d.Id,
                d.ApplicationId ?? Guid.Empty,
                d.Type.ToString(),
                d.FileName,
                d.FileUrl,
                d.FileSize,
                d.IsVerified,
                d.VerificationNotes,
                d.VerificationDate,
                d.CreatedAt,
                d.IsVerified ? "Verified" : "Pending Verification"
            )).ToList();

            return Result<List<DocumentDto>>.Success(documentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents for application {ApplicationId}", request.ApplicationId);
            return Result<List<DocumentDto>>.Failure("An error occurred while fetching documents");
        }
    }
}