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
    private readonly Option<LogEventLevel?> _logLevelOption =
        new("--LogLevel")
        {
            Required = false,
            Description = "Specifies the meaning and relative importance of a log event."
        };

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

    private readonly Option<SampleOptionsEnum2> _param3Option =
        new("--Param3")
        {
            Required = true,
            Description = "Enum Parameter 3"
        };

    public SampleCommand2() : base("Sample2", "Sample Command 2")
    {
        Options.Add(_param1Option);
        Options.Add(_param2Option);
        Options.Add(_param3Option);
        Options.Add(_logLevelOption);

        SetAction(CommandAction);
    }

    private async Task<int> CommandAction(ParseResult result)
    {
        var options = new SampleOptions2(result.GetRequiredValue(_param1Option), result.GetValue(_param2Option),
            result.GetRequiredValue(_param3Option), result.GetValue(_logLevelOption));

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