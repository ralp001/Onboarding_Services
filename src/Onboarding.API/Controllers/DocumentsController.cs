// Api/Controllers/DocumentsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onboarding.Application.Features.Documents.Commands.UploadDocument;
using Onboarding.Application.Features.Documents.Commands.VerifyDocument;
using Onboarding.Application.Features.Documents.Queries.GetDocumentsByApplication;
using Onboarding.Domain.Enums;

namespace Onboarding.Api.Controllers;

[Authorize]
[Route("api/documents")]
public class DocumentsController : BaseApiController
{
    /// <summary>
    /// Upload document for an application
    /// </summary>
    [Authorize(Roles = "Parent")]
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadDocumentCommand command)
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
    /// Verify document (admin only)
    /// </summary>
    [Authorize(Roles = "SuperAdmin,SchoolAdmin,AdmissionOfficer")]
    [HttpPost("{id}/verify")]
    public async Task<IActionResult> Verify(Guid id, [FromBody] VerifyDocumentRequest request)
    {
        // Get admin ID from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var adminId))
            return Unauthorized();

        var command = new VerifyDocumentCommand
        {
            DocumentId = id,
            VerifiedByUserId = adminId,
            IsVerified = request.IsVerified,
            VerificationNotes = request.VerificationNotes
        };

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Get documents for an application
    /// </summary>
    [HttpGet("application/{applicationId}")]
    public async Task<IActionResult> GetByApplication(Guid applicationId)
    {
        // Get user ID and role from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "role");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId) || roleClaim == null)
            return Unauthorized();

        var query = new GetDocumentsByApplicationQuery
        {
            ApplicationId = applicationId,
            UserId = userId,
            UserRole = Enum.Parse<UserRole>(roleClaim.Value)
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    // Request DTO for verify endpoint
    public class VerifyDocumentRequest
    {
        public bool IsVerified { get; set; } = true;
        public string? VerificationNotes { get; set; }
    }
}