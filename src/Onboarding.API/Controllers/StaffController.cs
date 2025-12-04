// Api/Controllers/StaffController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onboarding.Application.Features.Staff.Commands.CreateStaff;
using Onboarding.Application.Features.Staff.Commands.UpdateStaffStatus;
using Onboarding.Application.Features.Staff.Queries.GetStaffById;
using Onboarding.Application.Features.Staff.Queries.GetAvailableInterviewers;
using Onboarding.Domain.Enums;

namespace Onboarding.Api.Controllers;

[Authorize(Roles = "SuperAdmin,SchoolAdmin")]
[Route("api/staff")]
public class StaffController : BaseApiController
{
    /// <summary>
    /// Create new staff member
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateStaffCommand command)
    {
        // Get admin ID from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var adminId))
            return Unauthorized();

        command.CreatedByUserId = adminId;
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update staff status
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStaffStatusRequest request)
    {
        // Get admin ID from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var adminId))
            return Unauthorized();

        var command = new UpdateStaffStatusCommand
        {
            StaffId = id,
            UpdatedByUserId = adminId,
            Status = request.Status,
            Reason = request.Reason
        };

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Get staff by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // Get user ID and role from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "role");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId) || roleClaim == null)
            return Unauthorized();

        var query = new GetStaffByIdQuery
        {
            StaffId = id,
            UserId = userId,
            UserRole = Enum.Parse<UserRole>(roleClaim.Value)
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get available interviewers
    /// </summary>
    [Authorize(Roles = "SuperAdmin,SchoolAdmin,AdmissionOfficer")]
    [HttpGet("interviewers/available")]
    public async Task<IActionResult> GetAvailableInterviewers([FromQuery] DateTime? date)
    {
        // Get user ID and role from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "role");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId) || roleClaim == null)
            return Unauthorized();

        var query = new GetAvailableInterviewersQuery
        {
            UserId = userId,
            UserRole = Enum.Parse<UserRole>(roleClaim.Value),
            Date = date
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    // Request DTOs
    public class UpdateStaffStatusRequest
    {
        public StaffStatus Status { get; set; }
        public string? Reason { get; set; }
    }
}