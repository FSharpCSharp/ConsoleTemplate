using ConsoleApp.CommandLine.CreateDemo.Options;

namespace ConsoleApp.CommandLine.CreateDemo.Handler;

internal interface ICreateDemoHandler
{
    Task<bool> ExecuteAsync(CreateDemoOptions options, CancellationToken cancellationToken);
}