using ConsoleApp.CommandLine.Sample2.Options;

namespace ConsoleApp.CommandLine.Sample2.Handler;

internal interface ISampleHandler2
{
    Task<int> ExecuteAsync(SampleOptions2 options, CancellationToken cancellationToken);
}