// Infrastructure/Data/Configurations/StudentConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onboarding.Domain.Entities;
using Onboarding.Domain.ValueObjects;

namespace Onboarding.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

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

        builder.OwnsOne(s => s.FatherInfo, father =>
        {
            father.Property(f => f.FullName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("FatherFullName");

            father.Property(f => f.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("FatherPhoneNumber");

            father.Property(f => f.Email)
                .HasMaxLength(100)
                .HasColumnName("FatherEmail");

            father.Property(f => f.Occupation)
                .HasMaxLength(100)
                .HasColumnName("FatherOccupation");

            father.Property(f => f.Relationship)
                .HasMaxLength(50)
                .HasColumnName("FatherRelationship")
                .HasDefaultValue("Father");
        });

        builder.OwnsOne(s => s.MotherInfo, mother =>
        {
            mother.Property(m => m.FullName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("MotherFullName");

            mother.Property(m => m.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("MotherPhoneNumber");

            mother.Property(m => m.Email)
                .HasMaxLength(100)
                .HasColumnName("MotherEmail");

            mother.Property(m => m.Occupation)
                .HasMaxLength(100)
                .HasColumnName("MotherOccupation");

            mother.Property(m => m.Relationship)
                .HasMaxLength(50)
                .HasColumnName("MotherRelationship")
                .HasDefaultValue("Mother");
        });

        builder.OwnsOne(s => s.GuardianInfo, guardian =>
        {
            guardian.Property(g => g.FullName)
                .HasMaxLength(100)
                .HasColumnName("GuardianFullName");

            guardian.Property(g => g.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("GuardianPhoneNumber");

            guardian.Property(g => g.Email)
                .HasMaxLength(100)
                .HasColumnName("GuardianEmail");

            guardian.Property(g => g.Occupation)
                .HasMaxLength(100)
                .HasColumnName("GuardianOccupation");

            guardian.Property(g => g.Relationship)
                .HasMaxLength(50)
                .HasColumnName("GuardianRelationship");
        });

        // Enums
        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.SelectedStream)
            .HasConversion<int?>();

        // Indexes
        builder.HasIndex(s => s.Email)
            .IsUnique();

        builder.HasIndex(s => s.PhoneNumber)
            .IsUnique();

        builder.HasIndex(s => s.AdmissionNumber)
            .IsUnique()
            .HasFilter("[AdmissionNumber] IS NOT NULL");

        builder.HasIndex(s => s.ParentUserId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.SelectedStream);

        // Relationships
        builder.HasOne(s => s.ParentUser)
            .WithMany(u => u.Students)
            .HasForeignKey(s => s.ParentUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Applications)
            .WithOne(a => a.Student)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Documents)
            .WithOne(d => d.Student)
            .HasForeignKey(d => d.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Interviews)
            .WithOne(i => i.Student)
            .HasForeignKey(i => i.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}