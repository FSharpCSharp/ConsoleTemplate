using ConsoleApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp.Application.Extensions;

internal static class HostExtensions
{
    public static void MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        using var ctx = scope.ServiceProvider.GetRequiredService<DemoContext>();
        ctx.Database.Migrate();
    }
}