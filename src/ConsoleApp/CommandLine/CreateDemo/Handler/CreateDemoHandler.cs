using ConsoleApp.Application.Commands;
using ConsoleApp.CommandLine.CreateDemo.Options;
using MediatR;

namespace ConsoleApp.CommandLine.CreateDemo.Handler;

internal class CreateDemoHandler : ICreateDemoHandler
{
    private readonly IMediator _mediator;

    public CreateDemoHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<bool> ExecuteAsync(CreateDemoOptions options, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateDemoCommand(options.Title, options.Description),
            cancellationToken);
    }
}