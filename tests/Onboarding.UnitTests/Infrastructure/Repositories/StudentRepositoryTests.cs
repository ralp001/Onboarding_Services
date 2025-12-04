// Onboarding.UnitTests/Infrastructure/Repositories/StudentRepositoryTests.cs
using Microsoft.EntityFrameworkCore;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Infrastructure.Data;
using Onboarding.Infrastructure.Data.Repositories;
using Onboarding.UnitTests.Shared.TestHelpers;
using Xunit;

namespace Onboarding.UnitTests.Infrastructure.Repositories;

public class StudentRepositoryTests : IDisposable
{
    private readonly OnboardingDbContext _context;
    private readonly StudentRepository _repository;

    public StudentRepositoryTests()
    {
        _context = TestDbContextFactory.Create();
        _repository = new StudentRepository(_context);
    }

    public void Dispose()
    {
        TestDbContextFactory.Destroy(_context);
    }

    [Fact]
    public async Task GetByAdmissionNumberAsync_ReturnsStudent_WhenExists()
    {
        // Arrange
        var student = TestDataFactory.CreateTestStudent();
        student.AdmissionNumber = "ADM-2024-001";

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAdmissionNumberAsync("ADM-2024-001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(student.AdmissionNumber, result.AdmissionNumber);
    }

    [Fact]
    public async Task GetByStatusAsync_ReturnsStudentsWithMatchingStatus()
    {
        // Arrange
        var prospective = TestDataFactory.CreateTestStudent(
            id: Guid.NewGuid(),
            status: StudentStatus.Prospective);

        var admitted = TestDataFactory.CreateTestStudent(
            id: Guid.NewGuid(),
            status: StudentStatus.Admitted);

        await _context.Students.AddRangeAsync(prospective, admitted);
        await _context.SaveChangesAsync();

        // Act
        var prospectiveStudents = await _repository.GetByStatusAsync(StudentStatus.Prospective);

        // Assert
        Assert.Single(prospectiveStudents);
        Assert.Equal(prospective.Id, prospectiveStudents[0].Id);
    }

    [Fact]
    public async Task ExistsByEmailAsync_ReturnsTrue_WhenEmailExists()
    {
        // Arrange
        var student = TestDataFactory.CreateTestStudent();
        student.Email = "unique@example.com";

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsByEmailAsync("unique@example.com");

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingStudents()
    {
        // Arrange
        var student1 = TestDataFactory.CreateTestStudent(
            id: Guid.NewGuid());
        student1.FirstName = "Chinedu";
        student1.LastName = "Okoro";

        var student2 = TestDataFactory.CreateTestStudent(
            id: Guid.NewGuid());
        student2.FirstName = "Amina";
        student2.LastName = "Bello";

        await _context.Students.AddRangeAsync(student1, student2);
        await _context.SaveChangesAsync();

        // Act - Search by first name
        var results = await _repository.SearchAsync("Chinedu");

        // Assert
        Assert.Single(results);
        Assert.Equal("Chinedu", results[0].FirstName);
    }

    [Fact]
    public async Task GetByStreamAsync_ReturnsStudentsWithMatchingStream()
    {
        // Arrange
        var scienceStudent = TestDataFactory.CreateTestStudent(id: Guid.NewGuid());
        scienceStudent.SelectedStream = SecondaryStream.Science;

        var artsStudent = TestDataFactory.CreateTestStudent(id: Guid.NewGuid());
        artsStudent.SelectedStream = SecondaryStream.Arts;

        await _context.Students.AddRangeAsync(scienceStudent, artsStudent);
        await _context.SaveChangesAsync();

        // Act
        var scienceStudents = await _repository.GetByStreamAsync(SecondaryStream.Science);

        // Assert
        Assert.Single(scienceStudents);
        Assert.Equal(SecondaryStream.Science, scienceStudents[0].SelectedStream);
    }
}