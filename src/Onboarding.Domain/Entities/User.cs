// Domain/Entities/User.cs
using Onboarding.Domain.Common;
using Onboarding.Domain.Enums;

namespace Onboarding.Domain.Entities;

public class User : BaseEntity
{
    // Identity Information
    public string Email { get; internal set; } = string.Empty;
    public string PhoneNumber { get; internal set; } = string.Empty;

    // Authentication
    public string PasswordHash { get; internal set; } = string.Empty;
    public string? PasswordSalt { get; internal set; }

    // Personal Information
    public string FirstName { get; internal set; } = string.Empty;
    public string LastName { get; internal set; } = string.Empty;
    public string? MiddleName { get; internal set; }

    // Account Status
    public UserStatus Status { get; internal set; } = UserStatus.PendingVerification;
    public UserRole Role { get; internal set; } = UserRole.Parent;

    // Nigerian Context
    public string? StateOfOrigin { get; internal set; }
    public string? LocalGovernment { get; internal set; }

    // Verification
    public bool IsEmailVerified { get; internal set; }
    public bool IsPhoneVerified { get; internal set; }
    public DateTime? EmailVerifiedAt { get; internal set; }
    public DateTime? PhoneVerifiedAt { get; internal set; }

    // Security
    public DateTime? LastLoginAt { get; internal set; }
    public string? LastLoginIp { get; internal set; }
    public int FailedLoginAttempts { get; internal set; }
    public DateTime? LockoutEnd { get; internal set; }

    // Password Reset
    public string? PasswordResetToken { get; internal set; }
    public DateTime? PasswordResetTokenExpiry { get; internal set; }

    // Email Verification
    public string? EmailVerificationToken { get; internal set; }
    public DateTime? EmailVerificationTokenExpiry { get; internal set; }

    // Navigation Properties
    public virtual ICollection<Student> Students { get; internal set; } = []; // Parents can have multiple students
    public virtual Staff? Staff { get; internal set; } // If user is staff member
    public virtual ICollection<Application> Applications { get; internal set; } = []; // Applications created by user

    internal User() { }
}