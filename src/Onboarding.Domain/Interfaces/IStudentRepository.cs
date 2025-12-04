// Domain/Interfaces/IStudentRepository.cs
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;

namespace Onboarding.Domain.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    // Domain-specific queries ONLY
    Task<Student?> GetByAdmissionNumberAsync(string admissionNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByStatusAsync(StudentStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByClassLevelAsync(ClassLevel classLevel, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByStreamAsync(SecondaryStream stream, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    // Nigerian specific
    Task<IReadOnlyList<Student>> GetByStateOfOriginAsync(string state, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByLocalGovernmentAsync(string lga, CancellationToken cancellationToken = default);

    // Existence checks
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsByAdmissionNumberAsync(string admissionNumber, CancellationToken cancellationToken = default);
}