// Infrastructure/Data/Repositories/DocumentRepository.cs
using Microsoft.EntityFrameworkCore;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;
using Onboarding.Infrastructure.Data;

namespace Onboarding.Infrastructure.Data.Repositories;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(OnboardingDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Document>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.StudentId == studentId)
            .Include(d => d.Student)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.ApplicationId == applicationId)
            .Include(d => d.Application)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByTypeAsync(DocumentType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.Type == type)
            .Include(d => d.Student)
            .Include(d => d.Application)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetUnverifiedDocumentsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => !d.IsVerified)
            .Include(d => d.Student)
            .Include(d => d.Application)
            .OrderBy(d => d.UploadDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetVerifiedDocumentsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.IsVerified)
            .Include(d => d.Student)
            .Include(d => d.Application)
            .OrderByDescending(d => d.VerificationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Document?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.FileName == fileName, cancellationToken);
    }

    public async Task<bool> FileNameExistsAsync(string fileName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(d => d.FileName == fileName, cancellationToken);
    }

    public async Task DeleteAsync(Document document, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(document);
        await _context.SaveChangesAsync(cancellationToken);
    }
}