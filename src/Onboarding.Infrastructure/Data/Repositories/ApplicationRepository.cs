// Infrastructure/Data/Repositories/ApplicationRepository.cs
using Microsoft.EntityFrameworkCore;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Infrastructure.Data.Repositories;

public class ApplicationRepository : Repository<Domain.Entities.Application>, IApplicationRepository
{
    public ApplicationRepository(OnboardingDbContext context) : base(context)
    {
    }

    public async Task<Domain.Entities.Application?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Student)
                .ThenInclude(s => s.ParentUser)
            .Include(a => a.User)
            .Include(a => a.Documents)
            .Include(a => a.Interview)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Domain.Entities.Application>> GetByParentIdAsync(
        Guid parentId,
        ApplicationStatus? status = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.Student)
            .Where(a => a.UserId == parentId);

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Domain.Entities.Application>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Application?> GetByApplicationNumberAsync(string applicationNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.ApplicationNumber == applicationNumber, cancellationToken);
    }

    public async Task<List<Domain.Entities.Application>> GetAllAsync(
        ApplicationStatus? status = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByStudentAndYearAsync(Guid studentId, string academicYear, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(a => a.StudentId == studentId && a.AcademicYear == academicYear, cancellationToken);
    }
}