// Domain/Enums/UserRole.cs
namespace Onboarding.Domain.Enums;

public enum UserRole
{
    SuperAdmin = 1,     // System administrator
    SchoolAdmin = 2,    // School administrator
    AdmissionOfficer = 3, // Handles applications
    Teacher = 4,        // Teaching staff
    Parent = 5,         // Parent/Guardian
    Student = 6         // Student (when admitted)
}