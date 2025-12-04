// Domain/Interfaces/IDocumentRepository.cs
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;

namespace Onboarding.Domain.Interfaces;

public interface IDocumentRepository : IRepository<Document>
{
    // Domain-specific queries ONLY
    Task<IReadOnlyList<Document>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByTypeAsync(DocumentType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetUnverifiedDocumentsAsync(CancellationToken cancellationToken = default);

    // File operations
    Task<Document?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken = default);
    Task<bool> FileNameExistsAsync(string fileName, CancellationToken cancellationToken = default);

    // Delete is specific to documents (other entities might not need it)
    Task DeleteAsync(Document document, CancellationToken cancellationToken = default);
}