// Domain/Enums/InterviewType.cs
namespace Onboarding.Domain.Enums;

public enum InterviewType
{
    InPerson = 1,
    Virtual = 2
}

public enum InterviewStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    Rescheduled = 4
}