// Onboarding.UnitTests/Infrastructure/Repositories/UserRepositoryTests.cs
using Microsoft.EntityFrameworkCore;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Infrastructure.Data;
using Onboarding.Infrastructure.Data.Repositories;
using Onboarding.UnitTests.Shared.TestHelpers;
using Xunit;

namespace Onboarding.UnitTests.Infrastructure.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly OnboardingDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        _context = TestDbContextFactory.Create();
        _repository = new UserRepository(_context);
    }

    public void Dispose()
    {
        TestDbContextFactory.Destroy(_context);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenEmailExists()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(email: "test@example.com");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenEmailDoesNotExist()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByPhoneAsync_ReturnsUser_WhenPhoneExists()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(phoneNumber: "08012345678");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByPhoneAsync("08012345678");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.PhoneNumber, result.PhoneNumber);
    }

    [Fact]
    public async Task IsEmailTakenAsync_ReturnsTrue_WhenEmailExists()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(email: "taken@example.com");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsEmailTakenAsync("taken@example.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsEmailTakenAsync_ReturnsFalse_WhenEmailDoesNotExist()
    {
        // Act
        var result = await _repository.IsEmailTakenAsync("notfound@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetByStatusAsync_ReturnsOnlyUsersWithMatchingStatus()
    {
        // Arrange
        var activeUser = TestDataFactory.CreateTestUser(
            id: Guid.NewGuid(),
            email: "active@example.com",
            status: UserStatus.Active);

        var pendingUser = TestDataFactory.CreateTestUser(
            id: Guid.NewGuid(),
            email: "pending@example.com",
            status: UserStatus.PendingVerification);

        await _context.Users.AddRangeAsync(activeUser, pendingUser);
        await _context.SaveChangesAsync();

        // Act
        var activeUsers = await _repository.GetByStatusAsync(UserStatus.Active);

        // Assert
        Assert.Single(activeUsers);
        Assert.Equal(activeUser.Email, activeUsers[0].Email);
    }

    [Fact]
    public async Task GetByRoleAsync_ReturnsOnlyUsersWithMatchingRole()
    {
        // Arrange
        var parentUser = TestDataFactory.CreateTestUser(
            id: Guid.NewGuid(),
            email: "parent@example.com",
            role: UserRole.Parent);

        var studentUser = TestDataFactory.CreateTestUser(
            id: Guid.NewGuid(),
            email: "student@example.com",
            role: UserRole.Student);

        await _context.Users.AddRangeAsync(parentUser, studentUser);
        await _context.SaveChangesAsync();

        // Act
        var parents = await _repository.GetByRoleAsync(UserRole.Parent);

        // Assert
        Assert.Single(parents);
        Assert.Equal(parentUser.Email, parents[0].Email);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingUsers()
    {
        // Arrange
        var user1 = TestDataFactory.CreateTestUser(
            id: Guid.NewGuid(),
            email: "john@example.com",
            firstName: "John",
            lastName: "Doe");

        var user2 = TestDataFactory.CreateTestUser(
            id: Guid.NewGuid(),
            email: "jane@example.com",
            firstName: "Jane",
            lastName: "Smith");

        await _context.Users.AddRangeAsync(user1, user2);
        await _context.SaveChangesAsync();

        // Act - Search by first name
        var results = await _repository.SearchAsync("John");

        // Assert
        Assert.Single(results);
        Assert.Equal("John", results[0].FirstName);
    }

    [Fact]
    public async Task GetByEmailVerificationTokenAsync_ReturnsUser_WhenValidTokenExists()
    {
        // Arrange
        var token = "valid_token_123";
        var user = TestDataFactory.CreateTestUser(email: "verify@example.com");
        user.EmailVerificationToken = token;
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(1);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailVerificationTokenAsync(token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(token, result.EmailVerificationToken);
    }

    [Fact]
    public async Task GetByEmailVerificationTokenAsync_ReturnsNull_WhenTokenExpired()
    {
        // Arrange
        var token = "expired_token";
        var user = TestDataFactory.CreateTestUser(email: "expired@example.com");
        user.EmailVerificationToken = token;
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(-1); // Expired

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailVerificationTokenAsync(token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCountByRoleAsync_ReturnsCorrectCount()
    {
        // Arrange
        var parent1 = TestDataFactory.CreateTestUser(
            id: Guid.NewGuid(),
            email: "parent1@example.com",
            role: UserRole.Parent);

        var parent2 = TestDataFactory.CreateTestUser(
            id: Guid.NewGuid(),
            email: "parent2@example.com",
            role: UserRole.Parent);

        var student = TestDataFactory.CreateTestUser(
            id: Guid.NewGuid(),
            email: "student@example.com",
            role: UserRole.Student);

        await _context.Users.AddRangeAsync(parent1, parent2, student);
        await _context.SaveChangesAsync();

        // Act
        var parentCount = await _repository.GetCountByRoleAsync(UserRole.Parent);

        // Assert
        Assert.Equal(2, parentCount);
    }
}