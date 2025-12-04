// Infrastructure/Data/Configurations/ApplicationConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onboarding.Domain.Entities;

namespace Onboarding.Infrastructure.Data.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<Domain.Entities.Application>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Application> builder)
    {
        builder.ToTable("Applications");

        builder.HasKey(a => a.Id);

        // Properties
        builder.Property(a => a.ApplicationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.ReviewNotes)
            .HasMaxLength(1000);

        builder.Property(a => a.DecisionRemarks)
            .HasMaxLength(500);

        // Enums
        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.ApplyingForLevel)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.ApplyingForClass)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.PreferredStream)
            .HasConversion<int?>();

        // Dates
        builder.Property(a => a.SubmissionDate);
        builder.Property(a => a.ReviewDate);
        builder.Property(a => a.DecisionDate);

        // Indexes
        builder.HasIndex(a => a.ApplicationNumber)
            .IsUnique();

        builder.HasIndex(a => a.StudentId);
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.ApplyingForLevel);
        builder.HasIndex(a => a.ApplyingForClass);

        // Relationships
        builder.HasOne(a => a.Student)
            .WithMany(s => s.Applications)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.User)
            .WithMany(u => u.Applications)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Documents)
            .WithOne(d => d.Application)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // FIX: Application has MANY Interviews (not One-to-One)
        builder.HasOne(a => a.Interview)
            .WithOne(i => i.Application)
            .HasForeignKey<Interview>(i => i.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}