// Domain/Entities/Document.cs
using Onboarding.Domain.Common;
using Onboarding.Domain.Enums;

namespace Onboarding.Domain.Entities;

public class Document : BaseEntity
{
    // Document Information
    public string FileName { get; internal set; } = string.Empty;
    public string FilePath { get; internal set; } = string.Empty;
    public string FileUrl { get; internal set; } = string.Empty;
    public DocumentType Type { get; internal set; }
    public string ContentType { get; internal set; } = string.Empty;
    public long FileSize { get; internal set; }

    // Metadata
    public string? Description { get; internal set; }
    public DateTime UploadDate { get; internal set; }
    public bool IsVerified { get; internal set; }
    public string? VerificationNotes { get; internal set; }
    public DateTime? VerificationDate { get; internal set; }

    // Relationships
    public Guid? StudentId { get; internal set; }
    public Guid? ApplicationId { get; internal set; }
    public Guid? StaffId { get; internal set; }

    // Navigation Properties
    public virtual Student? Student { get; internal set; }
    public virtual Application? Application { get; internal set; }
    public virtual Staff? Staff { get; internal set; }

    internal Document() { }
}