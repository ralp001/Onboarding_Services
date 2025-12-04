// Api/Controllers/InterviewsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onboarding.Application.Features.Interviews.Commands.ScheduleInterview;
using Onboarding.Application.Features.Interviews.Commands.ConductInterview;
using Onboarding.Application.Features.Interviews.Queries.GetUpcomingInterviews;
using Onboarding.Application.Features.Interviews.Queries.GetInterviewDetails;
using Onboarding.Domain.Enums;

namespace Onboarding.Api.Controllers;

[Authorize]
[Route("api/interviews")]
public class InterviewsController : BaseApiController
{
    /// <summary>
    /// Schedule an interview (admin only)
    /// </summary>
    [Authorize(Roles = "SuperAdmin,SchoolAdmin,AdmissionOfficer")]
    [HttpPost("schedule")]
    public async Task<IActionResult> Schedule(ScheduleInterviewCommand command)
    {
        // Get admin ID from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var adminId))
            return Unauthorized();

        command.ScheduledByUserId = adminId;
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Conduct an interview (interviewer only)
    /// </summary>
    [Authorize(Roles = "Teacher")]
    [HttpPost("{id}/conduct")]
    public async Task<IActionResult> Conduct(Guid id, [FromBody] ConductInterviewRequest request)
    {
        // Get interviewer ID from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var interviewerId))
            return Unauthorized();

        var command = new ConductInterviewCommand
        {
            InterviewId = id,
            ConductedByUserId = interviewerId,
            Score = request.Score,
            Feedback = request.Feedback,
            Remarks = request.Remarks
        };

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Get upcoming interviews for current user
    /// </summary>
    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        // Get user ID and role from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "role");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId) || roleClaim == null)
            return Unauthorized();

        var query = new GetUpcomingInterviewsQuery
        {
            UserId = userId,
            UserRole = Enum.Parse<UserRole>(roleClaim.Value),
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get interview details
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // Get user ID and role from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "role");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId) || roleClaim == null)
            return Unauthorized();

        var query = new GetInterviewDetailsQuery
        {
            InterviewId = id,
            UserId = userId,
            UserRole = Enum.Parse<UserRole>(roleClaim.Value)
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    // Request DTO for conduct endpoint
    public class ConductInterviewRequest
    {
        public int Score { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }
}