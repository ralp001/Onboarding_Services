// Application/Features/Users/Commands/LoginUser/LoginUserCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Interfaces;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Entities;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Users.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginUserCommandHandler> _logger;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<LoginUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<LoginResponseDto>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get user by email
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                return Result<LoginResponseDto>.Failure("Invalid email or password");

            // 2. Check account status
            if (user.Status == UserStatus.Inactive || user.Status == UserStatus.Suspended)
                return Result<LoginResponseDto>.Failure("Account is inactive. Please contact support.");

            if (user.Status == UserStatus.Locked)
            {
                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
                    return Result<LoginResponseDto>.Failure($"Account locked until {user.LockoutEnd.Value:HH:mm}");

                // Auto-unlock if lockout expired
                user.Status = UserStatus.Active;
                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;
                await _userRepository.UpdateAsync(user, cancellationToken);
            }

            // 3. Verify password
            var isValidPassword = _passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash,
                user.PasswordSalt ?? string.Empty);

            if (!isValidPassword)
            {
                // Increment failed attempts
                user.FailedLoginAttempts++;

                // Lock account after 5 failed attempts
                if (user.FailedLoginAttempts >= 5)
                {
                    user.Status = UserStatus.Locked;
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(30); // 30 minute lockout
                    _logger.LogWarning("Account {Email} locked due to multiple failed attempts", user.Email);
                }

                await _userRepository.UpdateAsync(user, cancellationToken);
                return Result<LoginResponseDto>.Failure("Invalid email or password");
            }

            // 4. Reset failed attempts on successful login
            if (user.FailedLoginAttempts > 0)
            {
                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;
            }

            // 5. Update last login info
            user.LastLoginAt = DateTime.UtcNow;
            user.LastLoginIp = request.IpAddress;
            await _userRepository.UpdateAsync(user, cancellationToken);

            // 6. Generate JWT token
            var token = _tokenService.GenerateToken(user);

            // 7. Create response
            var response = new LoginResponseDto(
                user.Id,
                user.Email,
                $"{user.FirstName} {user.LastName}",
                token.Token,
                token.Expiry,
                user.Role,
                user.Status);

            _logger.LogInformation("User {Email} logged in successfully", user.Email);

            return Result<LoginResponseDto>.Success(response, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", request.Email);
            return Result<LoginResponseDto>.Failure("An error occurred during login");
        }
    }
}