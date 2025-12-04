// Domain/Entities/Student.cs
using Onboarding.Domain.Common;
using Onboarding.Domain.Enums;
using Onboarding.Domain.ValueObjects;

namespace Onboarding.Domain.Entities;

public class Student : BaseEntity
{
    // Personal Information
    public string FirstName { get; internal set; } = string.Empty;
    public string LastName { get; internal set; } = string.Empty;
    public string? MiddleName { get; internal set; }
    public DateTime DateOfBirth { get; internal set; }
    public string Gender { get; internal set; } = string.Empty;
    public string? Religion { get; internal set; }

    // Contact Information
    public string Email { get; internal set; } = string.Empty;
    public string PhoneNumber { get; internal set; } = string.Empty;
    public NigerianAddress Address { get; internal set; } = null!;

    // Academic Information
    public string? PreviousSchool { get; internal set; }
    public string? PreviousClass { get; internal set; }
    public string? AdmissionNumber { get; internal set; }
    public DateTime? AdmissionDate { get; internal set; }

    // Nigerian Context
    public string? StateOfOrigin { get; internal set; }
    public string? LocalGovernment { get; internal set; }
    public string? Nationality { get; internal set; } = "Nigerian";

    // Status
    public StudentStatus Status { get; internal set; } = StudentStatus.Prospective;

    // Parents/Guardians
    public ParentInfo FatherInfo { get; internal set; } = null!;
    public ParentInfo MotherInfo { get; internal set; } = null!;
    public ParentInfo? GuardianInfo { get; internal set; }

    // Stream Selection
    public SecondaryStream? SelectedStream { get; internal set; }
    public Guid? ParentUserId { get; internal set; }
    public virtual User? ParentUser { get; internal set; }

    // Navigation Properties
    public virtual ICollection<Application> Applications { get; internal set; } = [];
    public virtual ICollection<Document> Documents { get; internal set; } = [];
    public virtual ICollection<Interview> Interviews { get; internal set; } = [];

    // Private constructor - No methods!
    internal Student() { }
}