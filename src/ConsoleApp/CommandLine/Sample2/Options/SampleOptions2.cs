using Serilog.Events;

namespace ConsoleApp.CommandLine.Sample2.Options;

internal class SampleOptions2
{
    public SampleOptions2(string param1, int param2, SampleOptionsEnum2 param3, LogEventLevel? logLevel)
    {
        Param1 = param1;
        Param2 = param2;
        Param3 = param3;
        LogLevel = logLevel;
    }

    public string Param1 { get; }

    public int Param2 { get; }

    public SampleOptionsEnum2 Param3 { get; }

    public LogEventLevel? LogLevel { get; }
}