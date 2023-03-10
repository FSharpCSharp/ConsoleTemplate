using ConsoleAppTemplate.CommandLine.Sample2.Options;

namespace ConsoleAppTemplate.CommandLine.Sample2.Handler;

internal interface ISampleHandler2
{
    Task<int> ExecuteAsync(SampleOptions2 options, CancellationToken cancellationToken);
}