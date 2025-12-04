// Application/Common/Models/BaseCommand.cs
using MediatR;

namespace Onboarding.Application.Common.Models;

public abstract record BaseCommand<TResponse> : IRequest<TResponse>
{
    public Guid? RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}