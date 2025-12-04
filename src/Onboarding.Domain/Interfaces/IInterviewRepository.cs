// Domain/Interfaces/IInterviewRepository.cs
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;

namespace Onboarding.Domain.Interfaces;

public interface IInterviewRepository : IRepository<Interview>
{
    // Domain-specific queries ONLY
    Task<Interview?> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Interview>> GetByStatusAsync(InterviewStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Interview>> GetByInterviewerIdAsync(Guid interviewerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Interview>> GetByTypeAsync(InterviewType type, CancellationToken cancellationToken = default);

    // Scheduling
    Task<IReadOnlyList<Interview>> GetScheduledInterviewsAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Interview>> GetUpcomingInterviewsAsync(CancellationToken cancellationToken = default);

    // Conflicts
    Task<bool> HasSchedulingConflictAsync(Guid interviewerId, DateTime date, TimeSpan time, CancellationToken cancellationToken = default);
}