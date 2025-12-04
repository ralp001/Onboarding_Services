// Application/Features/Documents/Queries/GetDocumentsByApplication/GetDocumentsByApplicationQuery.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Documents.Queries.GetDocumentsByApplication;

public record GetDocumentsByApplicationQuery : IRequest<Result<List<DocumentDto>>>
{
    public Guid ApplicationId { get; init; }
    public Guid UserId { get; init; }
    public UserRole UserRole { get; init; }
}
public record DocumentDto(
    Guid DocumentId,
    Guid ApplicationId,
    string DocumentType,
    string FileName,
    string FileUrl,
    long FileSize,
    bool IsVerified,
    string? VerificationNotes,
    DateTime? VerificationDate,
    DateTime UploadedDate,
    string Status);