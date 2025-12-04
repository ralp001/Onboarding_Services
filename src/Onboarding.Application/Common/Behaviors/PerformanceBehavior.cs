// Application/Common/Behaviors/PerformanceBehavior.cs
using MediatR;
using Microsoft.Extensions.Logging;

namespace Onboarding.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly TimeSpan _threshold = TimeSpan.FromSeconds(3);

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var startTime = DateTime.UtcNow;

        var response = await next();

        var elapsed = DateTime.UtcNow - startTime;

        if (elapsed > _threshold)
        {
            _logger.LogWarning(
                "Request {@RequestName} took {@ElapsedMilliseconds}ms (Threshold: {@Threshold}ms)",
                requestName, elapsed.TotalMilliseconds, _threshold.TotalMilliseconds);
        }
        else
        {
            _logger.LogDebug(
                "Request {@RequestName} took {@ElapsedMilliseconds}ms",
                requestName, elapsed.TotalMilliseconds);
        }

        return response;
    }
}