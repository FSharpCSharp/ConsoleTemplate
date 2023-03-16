using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using ConsoleApp.Application.Extensions;
using ConsoleApp.CommandLine.CreateDemo.Handler;
using ConsoleApp.CommandLine.CreateDemo.Options;
using ConsoleApp.CommandLine.Sample.Handler;
using ConsoleApp.CommandLine.Sample.Options;
using ConsoleApp.CommandLine.Sample2.Handler;
using ConsoleApp.CommandLine.Sample2.Options;
using ConsoleApp.Infrastructure;
using ConsoleApp.Infrastructure.AutofacModules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp;

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
                    host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
                    host.ConfigureServices((context, services) =>
                        {
                            var configurationRoot = context.Configuration;
                            services.AddOptions();
                            services.Configure<SampleHandlerOptions>(configurationRoot.GetSection("SampleHandler"));
                            services.AddDbContext<DemoContext>(options =>
                                {
                                    options.UseSqlite(configurationRoot["ConnectionString"],
                                        sqliteOptionsAction: sqliteOptions =>
                                        {
                                            sqliteOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                        });
                                },
                                ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                            );
                        })
                        .ConfigureLogging(logging =>
                        {
                            logging.ClearProviders();
                            logging.AddConsole();
                            logging.AddDebug();
                        })
                        .ConfigureContainer<ContainerBuilder>(builder =>
                        {
                            builder.RegisterModule(new MediatorModule());
                            builder.RegisterModule(new ApplicationModule());
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
        root.AddCommand(CreateDemoConsoleCommand());
        return new CommandLineBuilder(root);
    }

    private static Command CreateDemoConsoleCommand()
    {
        var result =
            new Command("CreateDemo", "CreateDemo Command")
            {
                new Option<string>("--Title", "Title")
                {
                    IsRequired = false
                },
                new Option<string>("--Description", "Description")
                {
                    IsRequired = false
                }
            };

        result.Handler = CommandHandler.Create(async (CreateDemoOptions options, IHost host, CancellationToken token) =>
        {
            host.MigrateDatabase();

            var serviceProvider = host.Services;
            var createDemoHandler = serviceProvider.GetRequiredService<ICreateDemoHandler>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(Program));

            try
            {
                var commandResult = await createDemoHandler.ExecuteAsync(options, token);
                return commandResult ? 0 : 1;
            }
            catch (OperationCanceledException)
            {
                logger.LogError("Terminated");
                return 1;
            }
        });
        return result;
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