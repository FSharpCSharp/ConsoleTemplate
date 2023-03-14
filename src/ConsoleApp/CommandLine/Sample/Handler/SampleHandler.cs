using ConsoleApp.CommandLine.Sample.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApp.CommandLine.Sample.Handler;

internal class SampleHandler : ISampleHandler
{
    private readonly ILogger<SampleHandler> _logger;
    private readonly SampleHandlerOptions _options;

    public SampleHandler(ILogger<SampleHandler> logger, IOptions<SampleHandlerOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<int> ExecuteAsync(SampleOptions options, CancellationToken cancellationToken)
    {
        return await Task.FromResult(10);
    }
}