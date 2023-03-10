namespace ConsoleAppTemplate.CommandLine.Sample.Options;

internal class SampleOptions
{
    public SampleOptions(string param1, int param2, SampleOptionsEnum param3)
    {
        Param1 = param1;
        Param2 = param2;
        Param3 = param3;
    }

    public string Param1 { get; }

    public int Param2 { get; }

    public SampleOptionsEnum Param3 { get; }
}