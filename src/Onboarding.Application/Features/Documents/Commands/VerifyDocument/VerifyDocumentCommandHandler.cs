// Application/Features/Documents/Commands/VerifyDocument/VerifyDocumentCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Documents.Commands.VerifyDocument;

public class VerifyDocumentCommandHandler : IRequestHandler<VerifyDocumentCommand, Result>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VerifyDocumentCommandHandler> _logger;

    public VerifyDocumentCommandHandler(
        IDocumentRepository documentRepository,
        IUserRepository userRepository,
        ILogger<VerifyDocumentCommandHandler> logger)
    {
        _documentRepository = documentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(VerifyDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate user verifying document (must be admin/admission officer)
            var verifier = await _userRepository.GetByIdAsync(request.VerifiedByUserId, cancellationToken);
            if (verifier == null)
                return Result.Failure("Verifier user not found");

            var isAuthorized = verifier.Role == UserRole.SuperAdmin ||
                              verifier.Role == UserRole.SchoolAdmin ||
                              verifier.Role == UserRole.AdmissionOfficer;

            if (!isAuthorized)
                return Result.Failure("Only administrators can verify documents");

            // 2. Get document
            var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
            if (document == null)
                return Result.Failure("Document not found");

            // 3. Update document verification status
            document.IsVerified = request.IsVerified;
            document.VerificationNotes = request.VerificationNotes;
            document.VerificationDate = DateTime.UtcNow;

            await _documentRepository.UpdateAsync(document, cancellationToken);

            _logger.LogInformation(
                "Document {DocumentId} verification status set to {IsVerified} by user {UserId}",
                request.DocumentId, request.IsVerified, request.VerifiedByUserId);

            // 4. Check if all documents for the application are now verified
            if (document.ApplicationId.HasValue && request.IsVerified)
            {
                await CheckApplicationDocumentCompletion(document.ApplicationId.Value, cancellationToken);
            }

            return Result.Success($"Document {(request.IsVerified ? "verified" : "rejected")} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying document {DocumentId}", request.DocumentId);
            return Result.Failure("An error occurred while verifying document");
        }
    }

    private async Task CheckApplicationDocumentCompletion(Guid applicationId, CancellationToken cancellationToken)
    {
        try
        {
            var documents = await _documentRepository.GetByApplicationIdAsync(applicationId, cancellationToken);

            if (documents.All(d => d.IsVerified) && documents.Any())
            {
                _logger.LogInformation(
                    "All documents for application {ApplicationId} are now verified",
                    applicationId);

                // TODO: Update application status or send notification
                // Application is ready for review if all documents verified
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking document completion for application {ApplicationId}", applicationId);
        }
    }
}