// Api/Controllers/StudentsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onboarding.Application.Features.Students.Commands.CreateStudent;
using Onboarding.Application.Features.Students.Commands.UpdateStudent;
using Onboarding.Application.Features.Students.Queries.GetStudentById;
using Onboarding.Application.Features.Students.Queries;
using Onboarding.Application.Features.Students.Queries;
using Onboarding.Domain.Enums;

namespace Onboarding.Api.Controllers;

[Authorize]
[Route("api/students")]
public class StudentsController : BaseApiController
{
    /// <summary>
    /// Create a new student (parent only)
    /// </summary>
    [Authorize(Roles = "Parent")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentCommand command)
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
    /// Update student information
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateStudentCommand command)
    {
        // Get user ID and role from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "role");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId) || roleClaim == null)
            return Unauthorized();

        command.StudentId = id;
        command.UserId = userId;
        command.UserRole = Enum.Parse<UserRole>(roleClaim.Value);

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Get student by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // Get user ID and role from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "role");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId) || roleClaim == null)
            return Unauthorized();

        var query = new GetStudentByIdQuery
        {
            StudentId = id,
            UserId = userId,
            UserRole = Enum.Parse<UserRole>(roleClaim.Value)
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all students for current parent
    /// </summary>
    [Authorize(Roles = "Parent")]
    [HttpGet("my-students")]
    public async Task<IActionResult> GetMyStudents()
    {
        // Get parent ID from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var parentId))
            return Unauthorized();

        var query = new GetStudentsByParentQuery
        {
            ParentId = parentId,
            UserId = parentId,
            UserRole = UserRole.Parent
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all students (admin only)
    /// </summary>
    [Authorize(Roles = "SuperAdmin,SchoolAdmin,AdmissionOfficer")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllStudents([FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        // Get user ID and role from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "role");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId) || roleClaim == null)
            return Unauthorized();

        var query = new GetAllStudentsQuery
        {
            UserId = userId,
            UserRole = Enum.Parse<UserRole>(roleClaim.Value),
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}