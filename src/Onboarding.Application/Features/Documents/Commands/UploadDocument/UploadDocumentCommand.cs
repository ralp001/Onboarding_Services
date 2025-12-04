// Application/Features/Documents/Commands/UploadDocument/UploadDocumentCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Documents.Commands.UploadDocument;

public record UploadDocumentCommand : IRequest<Result<Guid>>
{
    public Guid ApplicationId { get; init; }
    public Guid UserId { get; init; }
    public DocumentType Type { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string FileUrl { get; init; } = string.Empty;
    public string? Description { get; init; }
}