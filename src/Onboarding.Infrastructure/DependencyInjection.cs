// Infrastructure/DependencyInjection.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Onboarding.Application.Common.Interfaces;
using Onboarding.Domain.Interfaces;
using Onboarding.Infrastructure.Data;
using Onboarding.Infrastructure.Data.Repositories;
using Onboarding.Infrastructure.Services;

namespace Onboarding.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<OnboardingDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(OnboardingDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IInterviewRepository, InterviewRepository>();
        services.AddScoped<IStaffRepository, StaffRepository>();

        // Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IApplicationDbContext, OnboardingDbContext>();

        // Application DbContext Interface
        //services.AddScoped<Onboarding.Application.Common.Interfaces.IApplicationDbContext, OnboardingDbContext>();

        return services;
    }
}