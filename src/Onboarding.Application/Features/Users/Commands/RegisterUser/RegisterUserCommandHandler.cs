// Application/Features/Users/Commands/RegisterUser/RegisterUserCommandHandler.cs
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Interfaces;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        ILogger<RegisterUserCommandHandler> logger,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Check if email already exists
            var emailExists = await _userRepository.IsEmailTakenAsync(request.Email, cancellationToken);
            if (emailExists)
                return Result<Guid>.Failure("Email is already registered");

            // 2. Check if phone number already exists
            var phoneExists = await _userRepository.IsPhoneTakenAsync(request.PhoneNumber, cancellationToken);
            if (phoneExists)
                return Result<Guid>.Failure("Phone number is already registered");

            // 3. Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // 4. Generate email verification token
            var emailVerificationToken = GenerateVerificationToken();

            // 5. Create User Entity
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email.Trim().ToLower(),
                PhoneNumber = request.PhoneNumber.Trim(),
                PasswordHash = passwordHash,  // DIRECT STRING ASSIGNMENT
                PasswordSalt = null,  // Can be null
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                MiddleName = request.MiddleName?.Trim(),
                Role = request.Role,
                Status = UserStatus.PendingVerification,
                StateOfOrigin = request.StateOfOrigin?.Trim(),
                LocalGovernment = request.LocalGovernment?.Trim(),
                EmailVerificationToken = emailVerificationToken,
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(7), // 7 days expiry
                CreatedAt = DateTime.UtcNow
            };

            // 6. Save user to database
            await _userRepository.AddAsync(user, cancellationToken);

            _logger.LogInformation("User {UserId} registered with email {Email}", user.Id, user.Email);

            // TODO: Send verification email
            // await _emailService.SendVerificationEmail(user.Email, user.EmailVerificationToken);

            return Result<Guid>.Success(user.Id, "Registration successful. Please check your email for verification.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user with email {Email}", request.Email);
            return Result<Guid>.Failure("An error occurred during registration");
        }
    }

    private static string GenerateVerificationToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .Replace("=", "");
    }
}