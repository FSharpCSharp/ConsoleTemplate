using ConsoleApp.CommandLine.Sample.Handler;
using ConsoleApp.CommandLine.Sample2.Handler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ConsoleApp.HostBuilder;

internal static class HostBuilderHelper
{
    public const string SettingsFileName = "appsettings.json";

    public static IHostBuilder CreateHostBuilder(HostBuilderSettings settings)
    {
        return new Microsoft.Extensions.Hosting.HostBuilder()
            .ConfigureHostConfiguration(builder =>
            {
                List<string> arguments = new();
                builder.AddCommandLine(arguments.ToArray());
            })
            .ConfigureDefaults(null)
            .UseContentRoot(settings.ContentRootDirectory)
            .ConfigureAppConfiguration(config =>
            {
                config.SetBasePath(HostBuilderSettings.GetBasePath())
                    .AddJsonFile(SettingsFileName, false, true);
            })
            .UseDefaultServiceProvider((context, serviceProviderOptions) =>
            {
                var isDevelopment = context.HostingEnvironment.IsDevelopment();
                if (!isDevelopment)
                    return;

                serviceProviderOptions.ValidateScopes = false;
                serviceProviderOptions.ValidateOnBuild = false;
            })
            .ConfigureServices((context, services) =>
            {
                var configurationRoot = context.Configuration;
                services.AddOptions();
                services.Configure<SampleHandlerOptions>(configurationRoot.GetSection("SampleHandler"));
                services.AddSingleton<ISampleHandler, SampleHandler>();
                services.AddSingleton<ISampleHandler2, SampleHandler2>();
            })
            .UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration, HostBuilderSettings.Options)
                .Enrich.FromLogContext());
        ;
    }
}