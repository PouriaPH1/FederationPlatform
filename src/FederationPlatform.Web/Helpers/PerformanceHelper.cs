using System.Diagnostics;

namespace FederationPlatform.Web.Helpers;

public class PerformanceHelper
{
    private readonly Stopwatch _stopwatch;
    private readonly string _operationName;
    private readonly ILogger _logger;

    public PerformanceHelper(string operationName, ILogger logger)
    {
        _operationName = operationName;
        _logger = logger;
        _stopwatch = Stopwatch.StartNew();
    }

    public void Stop()
    {
        _stopwatch.Stop();
        var elapsed = _stopwatch.ElapsedMilliseconds;

        if (elapsed > 1000)
        {
            _logger.LogWarning("Performance: {Operation} took {Elapsed}ms (SLOW)", _operationName, elapsed);
        }
        else if (elapsed > 500)
        {
            _logger.LogInformation("Performance: {Operation} took {Elapsed}ms", _operationName, elapsed);
        }
        else
        {
            _logger.LogDebug("Performance: {Operation} took {Elapsed}ms", _operationName, elapsed);
        }
    }

    public static IDisposable Measure(string operationName, ILogger logger)
    {
        return new PerformanceMeasurement(operationName, logger);
    }

    private class PerformanceMeasurement : IDisposable
    {
        private readonly PerformanceHelper _helper;

        public PerformanceMeasurement(string operationName, ILogger logger)
        {
            _helper = new PerformanceHelper(operationName, logger);
        }

        public void Dispose()
        {
            _helper.Stop();
        }
    }
}
