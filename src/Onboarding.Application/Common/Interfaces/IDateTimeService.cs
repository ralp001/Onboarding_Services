// Application/Common/Interfaces/IDateTimeService.cs
namespace Onboarding.Application.Common.Interfaces;

public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}