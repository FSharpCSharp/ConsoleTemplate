using System.CommandLine;
using ConsoleApp.CommandLine.Sample2.Handler;
using ConsoleApp.CommandLine.Sample2.Options;
using ConsoleApp.HostBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace ConsoleApp.CommandLine.Sample2.Command;

internal class SampleCommand2 : System.CommandLine.Command
{
    public SampleCommand2() : base("Sample2", "Sample Command 2")
    {
        Add(Param1Option);
        Add(Param2Option);
        Add(Param3Option);
        Add(LogLevelOption);

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

    private static Option<SampleOptionsEnum2> Param3Option =>
        new("--Param3")
        {
            Required = true,
            Description = "Enum Parameter 3"
        };

    private static Option<LogEventLevel?> LogLevelOption =>
        new("--LogLevel")
        {
            Required = false,
            Description = "Specifies the meaning and relative importance of a log event."
        };

    private async Task<int> CommandAction(ParseResult result)
    {
        var options = new SampleOptions2(result.GetRequiredValue(Param1Option), result.GetValue(Param2Option),
            result.GetRequiredValue(Param3Option), result.GetValue(LogLevelOption));

        var settings = new HostBuilderSettings();
        var host = HostBuilderHelper.CreateHostBuilder(settings)
            .Build();

        if (options.LogLevel != null)
        {
            var controlSwitch = HostBuilderSettings.AllSwitches["$controlSwitch"];
            controlSwitch.MinimumLevel = (LogEventLevel)options.LogLevel;
        }

        var serviceProvider = host.Services;
        var sampleHandler = serviceProvider.GetRequiredService<ISampleHandler2>();
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