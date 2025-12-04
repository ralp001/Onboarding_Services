// Infrastructure/Data/OnboardingDbContext.cs
using Microsoft.EntityFrameworkCore;
using Onboarding.Application.Common.Interfaces; // ADD THIS
using Onboarding.Domain.Entities;
using Onboarding.Infrastructure.Data.Configurations;

namespace Onboarding.Infrastructure.Data;

// ADD: IApplicationDbContext after DbContext
public class OnboardingDbContext : DbContext, IApplicationDbContext
{
    public OnboardingDbContext(DbContextOptions<OnboardingDbContext> options)
        : base(options)
    {
    }

    // DbSets (these already match the interface)
    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Domain.Entities.Application> Applications => Set<Domain.Entities.Application>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<Staff> Staff => Set<Staff>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OnboardingDbContext).Assembly);
    }

    // SaveChangesAsync is already inherited from DbContext
}