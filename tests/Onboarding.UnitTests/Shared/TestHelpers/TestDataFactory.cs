// Onboarding.UnitTests/Shared/TestHelpers/TestDataFactory.cs
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.ValueObjects;

namespace Onboarding.UnitTests.Shared.TestHelpers;

public static class TestDataFactory
{
    public static User CreateTestUser(
        Guid? id = null,
        string email = "test@example.com",
        string phoneNumber = "08012345678",
        UserRole role = UserRole.Parent,
        UserStatus status = UserStatus.Active)
    {
        return new User
        {
            Id = id ?? Guid.NewGuid(),
            Email = email,
            PhoneNumber = phoneNumber,
            PasswordHash = "hashed_password",
            FirstName = "Test",
            LastName = "User",
            Role = role,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Student CreateTestStudent(
        Guid? id = null,
        Guid? parentUserId = null,
        StudentStatus status = StudentStatus.Prospective)
    {
        var address = new NigerianAddress(
            "123 Test Street",
            "Lagos",
            "Ikeja",
            NigerianState.Lagos,
            "100001");

        var fatherInfo = new ParentInfo(
            "John Doe",
            "08011111111",
            "father@example.com",
            "Engineer",
            "Father");

        var motherInfo = new ParentInfo(
            "Jane Doe",
            "08022222222",
            "mother@example.com",
            "Doctor",
            "Mother");

        return new Student
        {
            Id = id ?? Guid.NewGuid(),
            ParentUserId = parentUserId ?? Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Student",
            DateOfBirth = new DateTime(2010, 1, 1),
            Gender = "Male",
            Email = "student@example.com",
            PhoneNumber = "08033333333",
            Address = address,
            FatherInfo = fatherInfo,
            MotherInfo = motherInfo,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
    }
}