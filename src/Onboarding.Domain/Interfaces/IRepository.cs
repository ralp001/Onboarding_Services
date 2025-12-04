// Domain/Interfaces/IRepository.cs
using Onboarding.Domain.Common;

namespace Onboarding.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    // Basic CRUD operations that ALL entities need
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
}