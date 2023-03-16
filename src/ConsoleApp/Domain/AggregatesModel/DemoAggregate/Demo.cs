using ConsoleApp.Domain.SeedWork;

namespace ConsoleApp.Domain.AggregatesModel.DemoAggregate;

class Demo : Entity, IAggregateRoot
{
    public Demo(int id, string title, string description)
    {
        Id = id;
        if (string.IsNullOrEmpty(title)) throw new ArgumentException("Value cannot be null or empty.", nameof(title));
        Title = title;
        Description = description;
    }

    public string Title { get; private set; }

    public string Description { get; private set; }
}