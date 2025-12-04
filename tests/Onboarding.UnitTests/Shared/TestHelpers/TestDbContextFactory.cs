// Onboarding.UnitTests/Shared/TestHelpers/TestDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Onboarding.Infrastructure.Data;

namespace Onboarding.UnitTests.Shared.TestHelpers;

public static class TestDbContextFactory
{
    public static OnboardingDbContext Create()
    {
        var options = new DbContextOptionsBuilder<OnboardingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new OnboardingDbContext(options);
    }

    public static void Destroy(OnboardingDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}