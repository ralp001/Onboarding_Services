// Infrastructure/Data/Repositories/InterviewRepository.cs
using Microsoft.EntityFrameworkCore;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;
using Onboarding.Infrastructure.Data;

namespace Onboarding.Infrastructure.Data.Repositories;

public class InterviewRepository : Repository<Interview>, IInterviewRepository
{
    public InterviewRepository(OnboardingDbContext context) : base(context)
    {
    }

    public async Task<Interview?> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Application)
                .ThenInclude(a => a.Student)
            .Include(i => i.Interviewer)
            .FirstOrDefaultAsync(i => i.ApplicationId == applicationId, cancellationToken);
    }

    public async Task<IReadOnlyList<Interview>> GetByStatusAsync(InterviewStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.Status == status)
            .Include(i => i.Application)
                .ThenInclude(a => a.Student)
            .Include(i => i.Interviewer)
            .OrderBy(i => i.ScheduledDate)
            .ThenBy(i => i.ScheduledTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Interview>> GetByInterviewerIdAsync(Guid interviewerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.InterviewerId == interviewerId)
            .Include(i => i.Application)
                .ThenInclude(a => a.Student)
            .OrderBy(i => i.ScheduledDate)
            .ThenBy(i => i.ScheduledTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Interview>> GetByTypeAsync(InterviewType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.Type == type)
            .Include(i => i.Application)
                .ThenInclude(a => a.Student)
            .Include(i => i.Interviewer)
            .OrderBy(i => i.ScheduledDate)
            .ThenBy(i => i.ScheduledTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Interview>> GetScheduledInterviewsAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.ScheduledDate.Date == date.Date &&
                       i.Status == InterviewStatus.Scheduled)
            .Include(i => i.Application)
                .ThenInclude(a => a.Student)
            .Include(i => i.Interviewer)
            .OrderBy(i => i.ScheduledTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Interview>> GetUpcomingInterviewsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        return await _dbSet
            .Where(i => i.ScheduledDate >= today &&
                       i.Status == InterviewStatus.Scheduled)
            .Include(i => i.Application)
                .ThenInclude(a => a.Student)
            .Include(i => i.Interviewer)
            .OrderBy(i => i.ScheduledDate)
            .ThenBy(i => i.ScheduledTime)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Interview>> GetCompletedInterviewsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.Status == InterviewStatus.Completed)
            .Include(i => i.Application)
                .ThenInclude(a => a.Student)
            .Include(i => i.Interviewer)
            .OrderByDescending(i => i.ConductedDate)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasSchedulingConflictAsync(Guid interviewerId, DateTime date, TimeSpan time, CancellationToken cancellationToken = default)
    {
        // Check if interviewer already has an interview scheduled at this time
        return await _dbSet
            .AnyAsync(i =>
                i.InterviewerId == interviewerId &&
                i.ScheduledDate.Date == date.Date &&
                i.ScheduledTime == time &&
                i.Status == InterviewStatus.Scheduled,
                cancellationToken);
    }
}