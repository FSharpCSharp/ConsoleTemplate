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
    public SampleCommand() : base("Sample", "Sample Command")
    {
        Add(Param1Option);
        Add(Param2Option);
        Add(Param3Option);

        SetAction(CommandAction);
    }

    private static Option<string> Param1Option =>
        new("--Param1")
        {
            Required = true,
            Description = "String Parameter 1"
        };

    private static Option<int> Param2Option =>
        new("--Param2")
        {
            Required = false,
            Description = "Integer Parameter 2"
        };

    private static Option<SampleOptionsEnum> Param3Option =>
        new("--Param3")
        {
            Required = true,
            Description = "Enum Parameter 3"
        };

    private async Task<int> CommandAction(ParseResult result)
    {
        var options = new SampleOptions(
            result.GetRequiredValue(Param1Option), result.GetValue(Param2Option),
            result.GetRequiredValue(Param3Option));

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