// Domain/Enums/ApplicationStatus.cs
namespace Onboarding.Domain.Enums;

public enum ApplicationStatus
{
    Draft = 1,
    Submitted = 2,
    UnderReview = 3,
    InterviewScheduled = 4,
    Approved = 5,
    Rejected = 6,
    Waitlisted = 7
}