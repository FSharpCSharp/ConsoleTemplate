namespace ConsoleApp.CommandLine.CreateDemo.Options;

internal class CreateDemoOptions
{
    public CreateDemoOptions(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public string Title { get; }

    public string Description { get; }
}