using Serilog.Core;
using Serilog.Settings.Configuration;

namespace ConsoleApp.HostBuilder;

internal sealed class HostBuilderSettings
{
    static HostBuilderSettings()
    {
        AllSwitches = new Dictionary<string, LoggingLevelSwitch>();
        Options = new ConfigurationReaderOptions
        {
            OnLevelSwitchCreated = (switchName, levelSwitch) => AllSwitches[switchName] = levelSwitch
        };
    }

    public string ContentRootDirectory { get; } = AppContext.BaseDirectory;

    public static Dictionary<string, LoggingLevelSwitch> AllSwitches { get; }

    public static ConfigurationReaderOptions Options { get; }

    internal static string GetBasePath()
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
}