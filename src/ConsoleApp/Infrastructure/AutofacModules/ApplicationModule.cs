using Autofac;
using ConsoleApp.CommandLine.CreateDemo.Handler;
using ConsoleApp.CommandLine.Sample.Handler;
using ConsoleApp.CommandLine.Sample2.Handler;
using ConsoleApp.Domain.AggregatesModel.DemoAggregate;
using ConsoleApp.Infrastructure.Repositories;

namespace ConsoleApp.Infrastructure.AutofacModules;

internal class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DemoRepository>()
            .As<IDemoRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<SampleHandler>()
            .As<ISampleHandler>()
            .SingleInstance();

        builder.RegisterType<SampleHandler2>()
            .As<ISampleHandler2>()
            .SingleInstance();

        builder.RegisterType<CreateDemoHandler>()
            .As<ICreateDemoHandler>()
            .SingleInstance();
    }
}