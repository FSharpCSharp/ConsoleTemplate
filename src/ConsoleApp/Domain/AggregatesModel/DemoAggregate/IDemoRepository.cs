using ConsoleApp.Domain.SeedWork;

namespace ConsoleApp.Domain.AggregatesModel.DemoAggregate;

interface IDemoRepository : IRepository<Demo>
{
    Demo Add(Demo demo);

    void Update(Demo demo);

    Task<Demo> GetAsync(int demoId);
}