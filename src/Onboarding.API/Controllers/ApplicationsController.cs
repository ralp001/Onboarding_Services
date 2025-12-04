// Api/Controllers/ApplicationsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onboarding.Application.Features.Applications.Commands.SubmitApplication;
using Onboarding.Application.Features.Applications.Queries.GetApplicationStatus;
using Onboarding.Application.Features.Applications.Queries.GetApplicationsByParent;
using Onboarding.Domain.Enums;

namespace Onboarding.Api.Controllers;

[Authorize]
[Route("api/applications")]
public class ApplicationsController : BaseApiController
{
    /// <summary>
    /// Submit a new application
    /// </summary>
    [Authorize(Roles = "Parent")]
    [HttpPost]
    public async Task<IActionResult> Submit(SubmitApplicationCommand command)
    {
        // Get parent ID from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var parentId))
            return Unauthorized();

        command.UserId = parentId;
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Get application status
    /// </summary>
    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        // Get user ID and role from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "role");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId) || roleClaim == null)
            return Unauthorized();

        var query = new GetApplicationStatusQuery
        {
            ApplicationId = id,
            UserId = userId,
            UserRole = Enum.Parse<UserRole>(roleClaim.Value)
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all applications for current parent
    /// </summary>
    [Authorize(Roles = "Parent")]
    [HttpGet("my-applications")]
    public async Task<IActionResult> GetMyApplications([FromQuery] ApplicationStatus? status)
    {
        // Get parent ID from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var parentId))
            return Unauthorized();

        var query = new GetApplicationsByParentQuery
        {
            ParentId = parentId,
            UserId = parentId,
            UserRole = UserRole.Parent,
            Status = status
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all applications (admin only)
    /// </summary>
    [Authorize(Roles = "SuperAdmin,SchoolAdmin,AdmissionOfficer")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllApplications([FromQuery] ApplicationStatus? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetAllApplicationsQuery
        {
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}