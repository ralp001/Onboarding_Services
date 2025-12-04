// Application/Features/Staff/Commands/UpdateStaffStatus/UpdateStaffStatusCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Onboarding.Application.Common.Models;
using Onboarding.Domain.Enums;
using Onboarding.Domain.Interfaces;

namespace Onboarding.Application.Features.Staff.Commands.UpdateStaffStatus;

public class UpdateStaffStatusCommandHandler : IRequestHandler<UpdateStaffStatusCommand, Result>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateStaffStatusCommandHandler> _logger;

    public UpdateStaffStatusCommandHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        ILogger<UpdateStaffStatusCommandHandler> logger)
    {
        _staffRepository = staffRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UpdateStaffStatusCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate admin user updating status
            var adminUser = await _userRepository.GetByIdAsync(request.UpdatedByUserId, cancellationToken);
            if (adminUser == null)
                return Result.Failure("Admin user not found");

            var isAuthorized = adminUser.Role == UserRole.SuperAdmin ||
                              adminUser.Role == UserRole.SchoolAdmin;

            if (!isAuthorized)
                return Result.Failure("Only school administrators can update staff status");

            // 2. Get staff
            var staff = await _staffRepository.GetByIdAsync(request.StaffId, cancellationToken);
            if (staff == null)
                return Result.Failure("Staff not found");

            // 3. Validate status transition
            if (staff.Status == request.Status)
                return Result.Failure($"Staff is already {request.Status.ToString().ToLower()}");

            // 4. Update status
            var previousStatus = staff.Status;
            staff.Status = request.Status;
            staff.UpdatedAt = DateTime.UtcNow;
            staff.UpdatedBy = request.UpdatedByUserId;

            await _staffRepository.UpdateAsync(staff, cancellationToken);

            _logger.LogInformation(
                "Staff {StaffId} status changed from {PreviousStatus} to {NewStatus} by admin {AdminId}",
                staff.Id, previousStatus, request.Status, request.UpdatedByUserId);

            // 5. TODO: Update user account status if staff has system access

            return Result.Success($"Staff status updated to {request.Status.ToString().ToLower()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff status for {StaffId}", request.StaffId);
            return Result.Failure("An error occurred while updating staff status");
        }
    }
}