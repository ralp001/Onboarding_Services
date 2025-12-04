// Infrastructure/Data/Configurations/UserConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onboarding.Domain.Entities;

namespace Onboarding.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.MiddleName)
            .HasMaxLength(50);

        // Enums
        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique();

        builder.HasIndex(u => u.EmailVerificationToken)
            .IsUnique()
            .HasFilter("[EmailVerificationToken] IS NOT NULL");

        builder.HasIndex(u => u.PasswordResetToken)
            .IsUnique()
            .HasFilter("[PasswordResetToken] IS NOT NULL");

        // Navigation Properties
        builder.HasMany(u => u.Students)
            .WithOne(s => s.ParentUser)
            .HasForeignKey(s => s.ParentUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Staff)
            .WithOne(s => s.User)
            .HasForeignKey<Staff>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Applications)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}