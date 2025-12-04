// Infrastructure/Data/Configurations/DocumentConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onboarding.Domain.Entities;

namespace Onboarding.Infrastructure.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        // Properties
        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.FileUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.Property(d => d.VerificationNotes)
            .HasMaxLength(1000);

        // Enums
        builder.Property(d => d.Type)
            .IsRequired()
            .HasConversion<int>();

        // File Size
        builder.Property(d => d.FileSize)
            .IsRequired();

        // Dates
        builder.Property(d => d.UploadDate)
            .IsRequired();

        builder.Property(d => d.VerificationDate);

        // Boolean
        builder.Property(d => d.IsVerified)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(d => d.FileName)
            .IsUnique();

        builder.HasIndex(d => d.StudentId);
        builder.HasIndex(d => d.ApplicationId);
        builder.HasIndex(d => d.StaffId);
        builder.HasIndex(d => d.Type);
        builder.HasIndex(d => d.IsVerified);

        // Relationships
        builder.HasOne(d => d.Student)
            .WithMany(s => s.Documents)
            .HasForeignKey(d => d.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Application)
            .WithMany(a => a.Documents)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Staff)
            .WithMany(s => s.Documents)
            .HasForeignKey(d => d.StaffId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}