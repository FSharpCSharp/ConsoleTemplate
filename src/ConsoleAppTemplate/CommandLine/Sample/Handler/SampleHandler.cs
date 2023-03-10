using ConsoleAppTemplate.CommandLine.Sample.Options;
using Microsoft.Extensions.Logging;

namespace ConsoleAppTemplate.CommandLine.Sample.Handler;

internal class SampleHandler : ISampleHandler
{
    private readonly ILogger<SampleHandler> _logger;

    public SampleHandler(ILogger<SampleHandler> logger)
    {
        _logger = logger;
    }

    public async Task<int> ExecuteAsync(SampleOptions options, CancellationToken cancellationToken)
    {
        return await Task.FromResult(10);
    }
}