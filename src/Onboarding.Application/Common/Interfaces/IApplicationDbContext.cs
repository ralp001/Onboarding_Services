// Keep only ONE of these (delete the duplicate):
// Application/Common/Interfaces/IApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Onboarding.Domain.Entities;

namespace Onboarding.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Student> Students { get; }
    DbSet<Domain.Entities.Application> Applications { get; }
    DbSet<Document> Documents { get; }
    DbSet<Interview> Interviews { get; }
    DbSet<Staff> Staff { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}