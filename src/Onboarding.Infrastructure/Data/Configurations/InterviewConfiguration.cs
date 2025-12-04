// Infrastructure/Data/Configurations/InterviewConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onboarding.Domain.Entities;

namespace Onboarding.Infrastructure.Data.Configurations;

public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.ToTable("Interviews");

        builder.HasKey(i => i.Id);

        // Properties
        builder.Property(i => i.MeetingLink)
            .HasMaxLength(500);

        builder.Property(i => i.MeetingId)
            .HasMaxLength(100);

        builder.Property(i => i.InterviewerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Feedback)
            .HasMaxLength(1000);

        builder.Property(i => i.Remarks)
            .HasMaxLength(500);

        // Enums
        builder.Property(i => i.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<int>();

        // Dates and Times
        builder.Property(i => i.ScheduledDate)
            .IsRequired();

        builder.Property(i => i.ScheduledTime)
            .IsRequired();

        builder.Property(i => i.ConductedDate);

        // Score
        builder.Property(i => i.Score);

        // Indexes
        builder.HasIndex(i => i.ApplicationId)
            .IsUnique(); // One interview per application

        builder.HasIndex(i => i.InterviewerId);
        builder.HasIndex(i => i.ScheduledDate);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.Type);

        // Relationships
        builder.HasOne(i => i.Application)
            .WithOne(a => a.Interview)
            .HasForeignKey<Interview>(i => i.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Interviewer)
            .WithMany(s => s.Interviews)
            .HasForeignKey(i => i.InterviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Student)
            .WithMany(s => s.Interviews)
            .HasForeignKey("StudentId") // Shadow property or add to entity
            .OnDelete(DeleteBehavior.Restrict);
    }
}