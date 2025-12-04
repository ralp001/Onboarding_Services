// Api/Controllers/BaseApiController.cs
using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Onboarding.Domain.Entities;

namespace Onboarding.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected ActionResult HandleResult<T>(Application.Common.Models.Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(new
        {
            Success = false,
            Message = result.Error,
            Errors = result.Errors
        });
    }

    protected ActionResult HandleResult(Application.Common.Models.Result result)
    {
        if (result.IsSuccess)
        {
            return Ok(new { Success = true, Message = result.Message });
        }

        return BadRequest(new
        {
            Success = false,
            Message = result.Error,
            Errors = result.Errors
        });
    }
}