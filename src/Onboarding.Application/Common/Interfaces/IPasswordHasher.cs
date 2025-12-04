// Application/Common/Interfaces/IPasswordHasher.cs
namespace Onboarding.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}