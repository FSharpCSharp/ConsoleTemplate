using System.CommandLine;
using ConsoleApp.CommandLine.Sample.Handler;
using ConsoleApp.CommandLine.Sample.Options;
using ConsoleApp.HostBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.CommandLine.Sample.Command;

internal class SampleCommand : System.CommandLine.Command
{
    private readonly Option<string> _param1Option =
        new("--Param1")
        {
            Required = true,
            Description = "String Parameter 1"
        };

    private readonly Option<int> _param2Option =
        new("--Param2")
        {
            Required = false,
            Description = "Integer Parameter 2"
        };

    private readonly Option<SampleOptionsEnum> _param3Option =
        new("--Param3")
        {
            Required = true,
            Description = "Enum Parameter 3"
        };

    public SampleCommand() : base("Sample", "Sample Command")
    {
        Options.Add(_param1Option);
        Options.Add(_param2Option);
        Options.Add(_param3Option);

        SetAction(CommandAction);
    }

    private async Task<int> CommandAction(ParseResult result)
    {
        var options = new SampleOptions(
            result.GetRequiredValue(_param1Option), result.GetValue(_param2Option),
            result.GetRequiredValue(_param3Option));

        var settings = new HostBuilderSettings();
        var host = HostBuilderHelper.CreateHostBuilder(settings)
            .Build();

        var serviceProvider = host.Services;
        var sampleHandler = serviceProvider.GetRequiredService<ISampleHandler>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(typeof(Program));
        var appLifetime =
            serviceProvider.GetRequiredService<IHostApplicationLifetime>();

        try
        {
            return await sampleHandler.ExecuteAsync(options, appLifetime.ApplicationStopping);
        }
        catch (OperationCanceledException)
        {
            logger.LogError("Terminated");
            return 1;
        }
    }
}