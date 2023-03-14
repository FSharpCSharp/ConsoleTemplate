using ConsoleApp.CommandLine.Sample2.Options;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.CommandLine.Sample2.Handler;

internal class SampleHandler2 : ISampleHandler2
{
    private readonly ILogger<SampleHandler2> _logger;

    public SampleHandler2(ILogger<SampleHandler2> logger)
    {
        _logger = logger;
    }

    public async Task<int> ExecuteAsync(SampleOptions2 options, CancellationToken cancellationToken)
    {
        return await Task.FromResult(10);
    }
}