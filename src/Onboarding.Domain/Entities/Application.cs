// Domain/Entities/Application.cs
using Onboarding.Domain.Common;
using Onboarding.Domain.Enums;

namespace Onboarding.Domain.Entities;

public class Application : BaseEntity
{
    // Basic Information
    public string ApplicationNumber { get; internal set; } = string.Empty;
    public Guid StudentId { get; internal set; }

    // Academic Information
    public EducationalLevel ApplyingForLevel { get; internal set; }
    public ClassLevel ApplyingForClass { get; internal set; }
    public SecondaryStream? PreferredStream { get; internal set; }

    // Status
    public ApplicationStatus Status { get; internal set; } = ApplicationStatus.Draft;

    // Dates
    public DateTime SubmissionDate { get; internal set; }
    public DateTime? ReviewDate { get; internal set; }
    public DateTime? DecisionDate { get; internal set; }

    // Review Information
    public string? ReviewNotes { get; internal set; }
    public string? DecisionRemarks { get; internal set; }

    // Navigation Properties
    public virtual Student Student { get; internal set; } = null!;
    public virtual ICollection<Document> Documents { get; internal set; } = [];
    public virtual Interview? Interview { get; internal set; }
    public Guid UserId { get; internal set; }
    public virtual User User { get; internal set; } = null!;
    public string AcademicYear { get; set; } = string.Empty;

    // Private constructor
    internal Application() { }
}