using ConsoleAppTemplate.CommandLine.Sample.Options;

namespace ConsoleAppTemplate.CommandLine.Sample.Handler;

internal interface ISampleHandler
{
    Task<int> ExecuteAsync(SampleOptions options, CancellationToken cancellationToken);
}