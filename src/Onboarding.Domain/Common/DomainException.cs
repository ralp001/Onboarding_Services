// Domain/Common/DomainException.cs
namespace Onboarding.Domain.Common;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}