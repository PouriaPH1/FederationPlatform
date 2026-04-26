using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FederationPlatform.Web.HealthChecks;

public class DiskSpaceHealthCheck : IHealthCheck
{
    private readonly long _minimumFreeMegabytes;

    public DiskSpaceHealthCheck(long minimumFreeMegabytes = 1024)
    {
        _minimumFreeMegabytes = minimumFreeMegabytes;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var drives = DriveInfo.GetDrives()
                .Where(d => d.IsReady && d.DriveType == DriveType.Fixed);

            var insufficientDrives = new List<string>();

            foreach (var drive in drives)
            {
                var freeSpaceMB = drive.AvailableFreeSpace / 1024 / 1024;
                if (freeSpaceMB < _minimumFreeMegabytes)
                {
                    insufficientDrives.Add($"{drive.Name}: {freeSpaceMB}MB free");
                }
            }

            if (insufficientDrives.Any())
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"Low disk space: {string.Join(", ", insufficientDrives)}"));
            }

            return Task.FromResult(HealthCheckResult.Healthy("Sufficient disk space available"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Disk space check failed",
                exception: ex));
        }
    }
}
