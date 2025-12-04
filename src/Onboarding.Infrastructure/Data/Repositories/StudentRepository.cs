// Infrastructure/Data/Repositories/StudentRepository.cs
using Microsoft.EntityFrameworkCore;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;
using Onboarding.Infrastructure.Data;

namespace Onboarding.Infrastructure.Data.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(OnboardingDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetByAdmissionNumberAsync(string admissionNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.ParentUser)
            .Include(s => s.Applications)
            .FirstOrDefaultAsync(s => s.AdmissionNumber == admissionNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetByStatusAsync(StudentStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Status == status)
            .Include(s => s.ParentUser)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetByClassLevelAsync(ClassLevel classLevel, CancellationToken cancellationToken = default)
    {
        // Note: ClassLevel is not in Student entity yet - we'll need to add it
        // For now, we'll implement it when we add ClassLevel property
        return await Task.FromResult<IReadOnlyList<Student>>([]);
    }

    public async Task<IReadOnlyList<Student>> GetByStreamAsync(SecondaryStream stream, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.SelectedStream == stream)
            .Include(s => s.ParentUser)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s =>
                s.FirstName.Contains(searchTerm) ||
                s.LastName.Contains(searchTerm) ||
                s.MiddleName != null && s.MiddleName.Contains(searchTerm) ||
                s.Email.Contains(searchTerm) ||
                s.PhoneNumber.Contains(searchTerm) ||
                s.AdmissionNumber != null && s.AdmissionNumber.Contains(searchTerm))
            .Include(s => s.ParentUser)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetByStateOfOriginAsync(string state, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.StateOfOrigin == state)
            .Include(s => s.ParentUser)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetByLocalGovernmentAsync(string lga, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.LocalGovernment == lga)
            .Include(s => s.ParentUser)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(s => s.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(s => s.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<bool> ExistsByAdmissionNumberAsync(string admissionNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(s => s.AdmissionNumber == admissionNumber, cancellationToken);
    }
}