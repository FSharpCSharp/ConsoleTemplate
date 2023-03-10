using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using ConsoleAppTemplate.CommandLine.Sample.Handler;
using ConsoleAppTemplate.CommandLine.Sample.Options;
using ConsoleAppTemplate.CommandLine.Sample2.Handler;
using ConsoleAppTemplate.CommandLine.Sample2.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleAppTemplate;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
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
                        })
                        .ConfigureLogging(logging =>
                        {
                            logging.ClearProviders();
                            logging.AddConsole();
                            logging.AddDebug();
                        });
                })
            .UseDefaults()
            .Build()
            .InvokeAsync(args);
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
                }
            };

        result.Handler = CommandHandler.Create(async (SampleOptions2 options, IHost host, CancellationToken token) =>
        {
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