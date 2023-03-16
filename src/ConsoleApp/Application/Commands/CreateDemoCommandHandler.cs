using ConsoleApp.Domain.AggregatesModel.DemoAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Application.Commands;

internal class CreateDemoCommandHandler : IRequestHandler<CreateDemoCommand, bool>
{
    private readonly IDemoRepository _demoRepository;
    private readonly ILogger<CreateDemoCommandHandler> _logger;

    public CreateDemoCommandHandler(IDemoRepository demoRepository)
    {
        _demoRepository = demoRepository;
    }

    public async Task<bool> Handle(CreateDemoCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("----- Creating Demo - : ");

        var demo = new Demo(command.Id, command.Title, command.Description);

        _demoRepository.Add(demo);

        return await _demoRepository.UnitOfWork
            .SaveEntitiesAsync(cancellationToken);
    }
}