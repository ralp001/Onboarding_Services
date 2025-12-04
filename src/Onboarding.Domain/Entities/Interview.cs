// Domain/Entities/Interview.cs
using Onboarding.Domain.Common;
using Onboarding.Domain.Enums;

namespace Onboarding.Domain.Entities;

public class Interview : BaseEntity
{
    // Interview Details
    public Guid ApplicationId { get; internal set; }
    public DateTime ScheduledDate { get; internal set; }
    public TimeSpan ScheduledTime { get; internal set; }
    public InterviewType Type { get; internal set; } = InterviewType.InPerson;

    // Virtual Interview Details
    public string? MeetingLink { get; internal set; }
    public string? MeetingId { get; internal set; }

    // Participants
    public Guid? InterviewerId { get; internal set; }
    public string InterviewerName { get; internal set; } = string.Empty;

    // Status
    public InterviewStatus Status { get; internal set; } = InterviewStatus.Scheduled;

    // Results
    public int? Score { get; internal set; }
    public string? Feedback { get; internal set; }
    public string? Remarks { get; internal set; }
    public DateTime? ConductedDate { get; internal set; }

    // Navigation Properties
    public virtual Application Application { get; internal set; } = null!;
    public virtual Staff? Interviewer { get; internal set; }
    public Guid StudentId { get; internal set; }
    public virtual Student Student { get; internal set; } = null!;

    internal Interview() { }
}