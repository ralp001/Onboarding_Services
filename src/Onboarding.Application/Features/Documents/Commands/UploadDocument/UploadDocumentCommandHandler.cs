// Application/Features/Documents/Commands/UploadDocument/UploadDocumentCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Documents.Commands.UploadDocument;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Result<Guid>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UploadDocumentCommandHandler> _logger;

    public UploadDocumentCommandHandler(
        IDocumentRepository documentRepository,
        IApplicationRepository applicationRepository,
        IUserRepository userRepository,
        ILogger<UploadDocumentCommandHandler> logger)
    {
        _documentRepository = documentRepository;
        _applicationRepository = applicationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        UploadDocumentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate application exists
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken);
            if (application == null)
                return Result<Guid>.Failure("Application not found");

            // 2. Validate user has access (parent or admin)
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<Guid>.Failure("User not found");

            var hasAccess = user.Role == UserRole.SuperAdmin ||
                           user.Role == UserRole.SchoolAdmin ||
                           (user.Role == UserRole.Parent && application.UserId == request.UserId);

            if (!hasAccess)
                return Result<Guid>.Failure("Access denied");

            // 3. Check file size limit (10MB for Nigerian school documents)
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (request.FileSize > maxFileSize)
                return Result<Guid>.Failure($"File size exceeds maximum limit of {maxFileSize / (1024 * 1024)}MB");

            // 4. Check if filename already exists
            var fileNameExists = await _documentRepository.FileNameExistsAsync(request.FileName, cancellationToken);
            if (fileNameExists)
                return Result<Guid>.Failure("A file with this name already exists");

            // 5. Check if document type already uploaded for this application
            var existingDocuments = await _documentRepository.GetByApplicationIdAsync(
                request.ApplicationId, cancellationToken);

            var duplicateDocument = existingDocuments.FirstOrDefault(d => d.Type == request.Type);
            if (duplicateDocument != null)
                return Result<Guid>.Failure($"{request.Type} has already been uploaded for this application");

            // 6. Create document
            var document = new Document
            {
                Id = Guid.NewGuid(),
                ApplicationId = request.ApplicationId,
                StudentId = application.StudentId,
                Type = request.Type,
                FileName = request.FileName,
                FilePath = $"applications/{application.Id}/{request.Type}/{request.FileName}",
                FileUrl = request.FileUrl,
                ContentType = request.ContentType,
                FileSize = request.FileSize,
                Description = request.Description,
                UploadDate = DateTime.UtcNow,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.UserId
            };

            // 7. Save document
            await _documentRepository.AddAsync(document, cancellationToken);

            _logger.LogInformation(
                "Document {DocumentId} uploaded for application {ApplicationId} by user {UserId}",
                document.Id, request.ApplicationId, request.UserId);

            return Result<Guid>.Success(document.Id, "Document uploaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document for application {ApplicationId}", request.ApplicationId);
            return Result<Guid>.Failure("An error occurred while uploading document");
        }
    }
}