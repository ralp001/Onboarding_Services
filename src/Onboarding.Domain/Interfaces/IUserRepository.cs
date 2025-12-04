// Domain/Interfaces/IUserRepository.cs
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;

namespace Onboarding.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    // Authentication
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default);

    // Security
    Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsPhoneTakenAsync(string phoneNumber, CancellationToken cancellationToken = default);

    // Queries
    Task<IReadOnlyList<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    // Nigerian specific
    Task<IReadOnlyList<User>> GetByStateOfOriginAsync(string state, CancellationToken cancellationToken = default);

    // Parent specific
    Task<IReadOnlyList<User>> GetParentsWithStudentsAsync(CancellationToken cancellationToken = default);

    // Verification
    Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default);

    // Statistics
    Task<int> GetCountByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
}