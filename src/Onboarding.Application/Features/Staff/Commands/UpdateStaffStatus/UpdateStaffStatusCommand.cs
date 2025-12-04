// Application/Features/Staff/Commands/UpdateStaffStatus/UpdateStaffStatusCommand.cs
using MediatR;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;

namespace Onboarding.Application.Features.Staff.Commands.UpdateStaffStatus;

public record UpdateStaffStatusCommand : IRequest<Result>
{
    public Guid StaffId { get; init; }
    public StaffStatus Status { get; init; }
    public Guid UpdatedByUserId { get; init; }
    public string? Reason { get; init; }
}