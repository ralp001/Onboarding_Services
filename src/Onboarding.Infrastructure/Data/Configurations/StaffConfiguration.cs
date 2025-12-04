// Infrastructure/Data/Configurations/StaffConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onboarding.Domain.Entities;
using Onboarding.Domain.ValueObjects;

namespace Onboarding.Infrastructure.Data.Configurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staff");

        builder.HasKey(s => s.Id);

        // Properties
        builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.MiddleName)
            .HasMaxLength(50);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.StaffNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.StateOfOrigin)
            .HasMaxLength(50);

        builder.Property(s => s.LocalGovernment)
            .HasMaxLength(100);

        // Value Objects (Owned Entities)
        builder.OwnsOne(s => s.Address, address =>
        {
            address.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("AddressStreet");

            address.Property(a => a.City)
                .HasMaxLength(100)
                .HasColumnName("AddressCity");

            address.Property(a => a.LGA)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("AddressLGA");

            address.Property(a => a.State)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName("AddressState");

            address.Property(a => a.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("AddressPostalCode");
        });

        // Enums
        builder.Property(s => s.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.Department)
            .HasConversion<int?>();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<int>();

        // Dates
        builder.Property(s => s.DateOfBirth)
            .IsRequired();

        builder.Property(s => s.EmploymentDate)
            .IsRequired();

        // Indexes
        builder.HasIndex(s => s.StaffNumber)
            .IsUnique();

        builder.HasIndex(s => s.Email)
            .IsUnique();

        builder.HasIndex(s => s.PhoneNumber)
            .IsUnique();

        builder.HasIndex(s => s.UserId)
            .IsUnique()
            .HasFilter("[UserId] IS NOT NULL");

        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.Type);
        builder.HasIndex(s => s.Department);

        // Relationships
        builder.HasOne(s => s.User)
            .WithOne(u => u.Staff)
            .HasForeignKey<Staff>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Documents)
            .WithOne(d => d.Staff)
            .HasForeignKey(d => d.StaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Interviews)
            .WithOne(i => i.Interviewer)
            .HasForeignKey(i => i.InterviewerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}