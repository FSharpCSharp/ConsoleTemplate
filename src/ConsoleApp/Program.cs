using System.CommandLine;
using System.Text;
using ConsoleApp.CommandLine.Sample.Command;
using ConsoleApp.CommandLine.Sample2.Command;
using ConsoleApp.HostBuilder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ConsoleApp;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        CreateLogger();

        try
        {
            RootCommand root = new()
            {
                new SampleCommand(),
                new SampleCommand2()
            };

            return await root.Parse(args).InvokeAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static void CreateLogger()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(HostBuilderSettings.GetBasePath())
            .AddJsonFile(HostBuilderHelper.SettingsFileName, false, true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration, HostBuilderSettings.Options)
            .Enrich.FromLogContext()
            .CreateBootstrapLogger();
    }
}