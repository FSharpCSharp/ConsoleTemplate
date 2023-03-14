using ConsoleApp.CommandLine.Sample.Options;

namespace ConsoleApp.CommandLine.Sample.Handler;

internal interface ISampleHandler
{
    Task<int> ExecuteAsync(SampleOptions options, CancellationToken cancellationToken);
}