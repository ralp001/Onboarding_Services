// Infrastructure/Data/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;
using Onboarding.Infrastructure.Data;

namespace Onboarding.Infrastructure.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(OnboardingDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> IsPhoneTakenAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.Role == role)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u =>
                u.FirstName.Contains(searchTerm) ||
                u.LastName.Contains(searchTerm) ||
                u.Email.Contains(searchTerm) ||
                u.PhoneNumber.Contains(searchTerm))
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByStateOfOriginAsync(string state, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.StateOfOrigin == state)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetParentsWithStudentsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.Role == UserRole.Parent)
            .Include(u => u.Students)
            .Where(u => u.Students.Any())
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u =>
                u.EmailVerificationToken == token &&
                u.EmailVerificationTokenExpiry > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u =>
                u.PasswordResetToken == token &&
                u.PasswordResetTokenExpiry > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task<int> GetCountByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(u => u.Role == role, cancellationToken);
    }
}