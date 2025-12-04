// Domain/Interfaces/IStaffRepository.cs
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;

namespace Onboarding.Domain.Interfaces;

public interface IStaffRepository : IRepository<Staff>
{
    // Domain-specific queries ONLY
    Task<Staff?> GetByStaffNumberAsync(string staffNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Staff>> GetByStatusAsync(StaffStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Staff>> GetByTypeAsync(StaffType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Staff>> GetByDepartmentAsync(Department department, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Staff>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    // Nigerian specific
    Task<IReadOnlyList<Staff>> GetByStateOfOriginAsync(string state, CancellationToken cancellationToken = default);

    // Interviewers
    Task<IReadOnlyList<Staff>> GetAvailableInterviewersAsync(CancellationToken cancellationToken = default);
    Task<Staff?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    // Existence checks
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByStaffNumberAsync(string staffNumber, CancellationToken cancellationToken = default);
}