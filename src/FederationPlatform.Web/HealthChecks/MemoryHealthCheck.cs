using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FederationPlatform.Web.HealthChecks;

public class MemoryHealthCheck : IHealthCheck
{
    private readonly long _thresholdBytes;

    public MemoryHealthCheck(long thresholdBytes = 1024 * 1024 * 1024) // 1GB default
    {
        _thresholdBytes = thresholdBytes;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var allocatedMB = allocated / 1024 / 1024;

            var data = new Dictionary<string, object>
            {
                { "AllocatedMB", allocatedMB },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) }
            };

            if (allocated >= _thresholdBytes)
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"High memory usage: {allocatedMB}MB",
                    data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                $"Memory usage is normal: {allocatedMB}MB",
                data: data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Memory check failed",
                exception: ex));
        }
    }
}
