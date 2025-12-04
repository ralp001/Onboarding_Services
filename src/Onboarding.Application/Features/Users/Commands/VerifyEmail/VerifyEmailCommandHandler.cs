// Application/Features/Users/Commands/VerifyEmail/VerifyEmailCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Users.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VerifyEmailCommandHandler> _logger;

    public VerifyEmailCommandHandler(
        IUserRepository userRepository,
        ILogger<VerifyEmailCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get user by verification token
            var user = await _userRepository.GetByEmailVerificationTokenAsync(request.Token, cancellationToken);
            if (user == null)
                return Result.Failure("Invalid or expired verification token");

            // 2. Verify email
            user.IsEmailVerified = true;
            user.EmailVerifiedAt = DateTime.UtcNow;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;

            // 3. Activate account if this was pending verification
            if (user.Status == UserStatus.PendingVerification)
            {
                user.Status = UserStatus.Active;
            }

            // 4. Save changes
            await _userRepository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("Email verified for user {UserId}", user.Id);

            return Result.Success("Email verified successfully. Your account is now active.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying email with token {Token}", request.Token);
            return Result.Failure("An error occurred while verifying email");
        }
    }
}