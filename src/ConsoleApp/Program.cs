using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using ConsoleApp.CommandLine.Sample.Handler;
using ConsoleApp.CommandLine.Sample.Options;
using ConsoleApp.CommandLine.Sample2.Handler;
using ConsoleApp.CommandLine.Sample2.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Settings.Configuration;

namespace ConsoleApp;

internal class Program
{
    private static Dictionary<string, LoggingLevelSwitch>? _allSwitches;
    private static ConfigurationReaderOptions? _options;


    private static async Task<int> Main(string[] args)
    {
        CreateLogger();

        return await BuildCommandLine()
            .UseHost(_ => Host.CreateDefaultBuilder(),
                host =>
                {
                    host.ConfigureAppConfiguration(config =>
                    {
                        config.SetBasePath(GetBasePath()).AddJsonFile("appsettings.json", false, true);
                    });
                    host.ConfigureServices((context, services) =>
                    {
                        var configurationRoot = context.Configuration;
                        services.AddOptions();
                        services.Configure<SampleHandlerOptions>(configurationRoot.GetSection("SampleHandler"));
                        services.AddSingleton<ISampleHandler, SampleHandler>();
                        services.AddSingleton<ISampleHandler2, SampleHandler2>();
                    });
                    host.UseSerilog((context, services, configuration) => configuration
                        .ReadFrom.Configuration(context.Configuration, _options)
                        .Enrich.FromLogContext());
                })
            .UseDefaults()
            .Build()
            .InvokeAsync(args);
    }

    private static void CreateLogger()
    {
        _allSwitches = new Dictionary<string, LoggingLevelSwitch>();
        _options = new ConfigurationReaderOptions
        {
            OnLevelSwitchCreated = (switchName, levelSwitch) => _allSwitches[switchName] = levelSwitch
        };

        var configuration = new ConfigurationBuilder()
            .SetBasePath(GetBasePath())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration, _options)
            .Enrich.FromLogContext()
            .CreateBootstrapLogger();
    }

    private static string GetBasePath()
    {
        // ReSharper disable once JoinDeclarationAndInitializer
        string basePath;
#if DEBUG
        basePath = AppContext.BaseDirectory;
#else
            using var processModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
            basePath = System.IO.Path.GetDirectoryName(processModule?.FileName);
#endif
        if (Environment.CurrentDirectory.Equals(basePath, StringComparison.InvariantCultureIgnoreCase))
            return basePath;

        Environment.CurrentDirectory = basePath;

        return basePath;
    }

    private static CommandLineBuilder BuildCommandLine()
    {
        var root = new RootCommand();
        root.AddCommand(CreateSampleConsoleCommand());
        root.AddCommand(CreateSampleConsoleCommand2());
        return new CommandLineBuilder(root);
    }

    private static Command CreateSampleConsoleCommand2()
    {
        var result =
            new Command("Sample2", "Sample Command 2")
            {
                new Option<string>("--Param1", "String Parameter 1")
                {
                    IsRequired = true
                },
                new Option<int>("--Param2", "Integer Parameter 2")
                {
                    IsRequired = false
                },
                new Option<SampleOptionsEnum>("--Param3", "Enum Parameter 3")
                {
                    IsRequired = true
                },
                new Option<LogEventLevel?>("--LogLevel",
                    "Specifies the meaning and relative importance of a log event.")
                {
                    IsRequired = false
                }
            };

        result.Handler = CommandHandler.Create(async (SampleOptions2 options, IHost host, CancellationToken token) =>
        {
            if (options.LogLevel != null)
            {
                var controlSwitch = _allSwitches["$controlSwitch"];
                controlSwitch.MinimumLevel = (LogEventLevel)options.LogLevel;
            }

            var serviceProvider = host.Services;
            var sampleHandler = serviceProvider.GetRequiredService<ISampleHandler2>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(Program));

            try
            {
                return await sampleHandler.ExecuteAsync(options, token);
            }
            catch (OperationCanceledException)
            {
                logger.LogError("Terminated");
                return 1;
            }
        });
        return result;
    }

    private static Command CreateSampleConsoleCommand()
    {
        var result =
            new Command("Sample", "Sample Command")
            {
                new Option<string>("--Param1", "String Parameter 1")
                {
                    IsRequired = true
                },
                new Option<int>("--Param2", "Integer Parameter 2")
                {
                    IsRequired = false
                },
                new Option<SampleOptionsEnum>("--Param3", "Enum Parameter 3")
                {
                    IsRequired = true
                }
            };

        result.Handler = CommandHandler.Create(async (SampleOptions options, IHost host, CancellationToken token) =>
        {
            var serviceProvider = host.Services;
            var sampleHandler = serviceProvider.GetRequiredService<ISampleHandler>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(Program));

            try
            {
                return await sampleHandler.ExecuteAsync(options, token);
            }
            catch (OperationCanceledException)
            {
                logger.LogError("Terminated");
                return 1;
            }
        });
        return result;
    }
}