// Api/Controllers/UsersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onboarding.Application.Features.Users.Commands.RegisterUser;
using Onboarding.Application.Features.Users.Commands.LoginUser;
using Onboarding.Application.Features.Users.Commands.VerifyEmail;
using Onboarding.Application.Features.Users.Queries.GetUserProfile;
using Onboarding.Application.Features.Users.Commands.UpdateUserProfile;
using Onboarding.Domain.Enums;

namespace Onboarding.Api.Controllers;

[Route("api/users")]
public class UsersController : BaseApiController
{
    /// <summary>
    /// Register a new user (parent)
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// User login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Verify email with token
    /// </summary>
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        var command = new VerifyEmailCommand { Token = token };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Get user profile (requires authentication)
    /// </summary>
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        // Get user ID from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var query = new GetUserProfileQuery { UserId = userId };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Update user profile (requires authentication)
    /// </summary>
    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateUserProfileCommand command)
    {
        // Get user ID from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        command.UserId = userId;
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all users (admin only)
    /// </summary>
    [Authorize(Roles = "SuperAdmin,SchoolAdmin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers([FromQuery] string? searchTerm, [FromQuery] UserRole? role)
    {
        var query = new GetAllUsersQuery { SearchTerm = searchTerm, Role = role };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}