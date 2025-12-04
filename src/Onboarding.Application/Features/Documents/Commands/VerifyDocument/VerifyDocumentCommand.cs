// Application/Features/Documents/Commands/VerifyDocument/VerifyDocumentCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;

namespace Onboarding.Application.Features.Documents.Commands.VerifyDocument;

public record VerifyDocumentCommand : IRequest<Result>
{
    public Guid DocumentId { get; init; }
    public Guid VerifiedByUserId { get; init; } // Admin/Admission officer
    public bool IsVerified { get; init; } = true;
    public string? VerificationNotes { get; init; }
}