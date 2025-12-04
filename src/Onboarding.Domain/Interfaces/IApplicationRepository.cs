// Domain/Interfaces/IApplicationRepository.cs
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;

namespace Onboarding.Domain.Interfaces;

public interface IApplicationRepository : IRepository<Application>
{
    Task<Application?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Application>> GetByParentIdAsync(Guid parentId, ApplicationStatus? status, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<List<Application>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<Application?> GetByApplicationNumberAsync(string applicationNumber, CancellationToken cancellationToken = default);
    Task<List<Application>> GetAllAsync(ApplicationStatus? status, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<bool> ExistsByStudentAndYearAsync(Guid studentId, string academicYear, CancellationToken cancellationToken = default);
}