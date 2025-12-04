// Infrastructure/Data/Repositories/StaffRepository.cs
using Microsoft.EntityFrameworkCore;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;
using Onboarding.Infrastructure.Data;

namespace Onboarding.Infrastructure.Data.Repositories;

public class StaffRepository : Repository<Staff>, IStaffRepository
{
    public StaffRepository(OnboardingDbContext context) : base(context)
    {
    }

    public async Task<Staff?> GetByStaffNumberAsync(string staffNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.StaffNumber == staffNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Staff>> GetByStatusAsync(StaffStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Status == status)
            .Include(s => s.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Staff>> GetByTypeAsync(StaffType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Type == type)
            .Include(s => s.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Staff>> GetByDepartmentAsync(Department department, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Department == department)
            .Include(s => s.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Staff>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.User)
            .Where(s =>
                s.FirstName.Contains(searchTerm) ||
                s.LastName.Contains(searchTerm) ||
                s.MiddleName != null && s.MiddleName.Contains(searchTerm) ||
                s.StaffNumber.Contains(searchTerm) ||
                s.User.Email.Contains(searchTerm) ||
                s.User.PhoneNumber.Contains(searchTerm))
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Staff>> GetByStateOfOriginAsync(string state, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.StateOfOrigin == state)
            .Include(s => s.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Staff>> GetAvailableInterviewersAsync(CancellationToken cancellationToken = default)
    {
        // Get active staff who are not currently scheduled for interviews in next 2 hours
        var now = DateTime.UtcNow;
        var twoHoursFromNow = now.AddHours(2);

        var staffWithInterviews = await _context.Interviews
            .Where(i => i.ScheduledDate.Date == now.Date &&
                       i.ScheduledTime >= now.TimeOfDay &&
                       i.ScheduledTime <= twoHoursFromNow.TimeOfDay &&
                       i.Status == InterviewStatus.Scheduled)
            .Select(i => i.InterviewerId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await _dbSet
            .Where(s => s.Status == StaffStatus.Active &&
                       s.Type == StaffType.Teaching && // Only teachers can be interviewers
                       !staffWithInterviews.Contains(s.Id))
            .Include(s => s.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.User)
            .AnyAsync(s => s.User.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByStaffNumberAsync(string staffNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(s => s.StaffNumber == staffNumber, cancellationToken);
    }
}