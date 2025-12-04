// Domain/Entities/Staff.cs
using Onboarding.Domain.Common;
using Onboarding.Domain.Enums;
using Onboarding.Domain.ValueObjects;

namespace Onboarding.Domain.Entities;

public class Staff : BaseEntity
{
    // Personal Information
    public string FirstName { get; internal set; } = string.Empty;
    public string LastName { get; internal set; } = string.Empty;
    public string? MiddleName { get; internal set; }
    public DateTime DateOfBirth { get; internal set; }
    public string Gender { get; internal set; } = string.Empty;

    // Contact Information
    public string Email { get; internal set; } = string.Empty;
    public string PhoneNumber { get; internal set; } = string.Empty;
    public NigerianAddress Address { get; internal set; } = null!;

    // Employment Information
    public string StaffNumber { get; internal set; } = string.Empty;
    public StaffType Type { get; internal set; } = StaffType.Teaching;
    public Department? Department { get; internal set; }
    public DateTime EmploymentDate { get; internal set; }
    public string? Qualification { get; internal set; }

    // Nigerian Context
    public string? StateOfOrigin { get; internal set; }
    public string? LocalGovernment { get; internal set; }
    public Guid? UserId { get; internal set; }
    public virtual User? User { get; internal set; }

    // Status
    public StaffStatus Status { get; internal set; } = StaffStatus.Active;

    // Navigation Properties
    public virtual ICollection<Interview> Interviews { get; internal set; } = [];
    public virtual ICollection<Document> Documents { get; internal set; } = [];

    internal Staff() { }
}