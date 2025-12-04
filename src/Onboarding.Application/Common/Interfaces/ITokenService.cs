// Application/Common/Interfaces/ITokenService.cs
using Onboarding.Domain.Entities;

namespace Onboarding.Application.Common.Interfaces;

public interface ITokenService
{
    TokenResult GenerateToken(User user);
    bool ValidateToken(string token);
    Guid? GetUserIdFromToken(string token);
}

public record TokenResult(string Token, DateTime Expiry);