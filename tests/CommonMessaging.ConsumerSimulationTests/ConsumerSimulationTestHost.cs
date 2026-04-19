using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ArchiScrapper.CommonMessaging.ConsumerSimulationTests;

internal sealed class ConsumerSimulationTestHost<TPayload>
{
    private readonly ServiceCollection services = [];
    private IServiceProvider? serviceProvider;

    public ConsumerSimulationTestHost()
    {
        services
            .AddCommonMessagingCore()
            .AddRawEventProcessingFlow<TPayload>();
    }

    public ConsumerSimulationTestHost<TPayload> AddSingleton<TService>(TService instance)
        where TService : class
    {
        services.AddSingleton(instance);
        return this;
    }

    public ConsumerSimulationTestHost<TPayload> AddInfrastructureStep<TStep>()
        where TStep : class, IInfrastructureStep<TPayload>
    {
        services.AddSingleton<IInfrastructureStep<TPayload>, TStep>();
        return this;
    }

    public ConsumerSimulationTestHost<TPayload> AddBusinessStep<TStep>()
        where TStep : class, IBusinessStep<TPayload>
    {
        services.AddSingleton<IBusinessStep<TPayload>, TStep>();
        return this;
    }

    public ConsumerSimulationTestHost<TPayload> AddConsumer<TConsumer>()
        where TConsumer : class, IEventConsumer<TPayload>
    {
        services.AddSingleton<IEventConsumer<TPayload>, TConsumer>();
        return this;
    }

    public IServiceProvider BuildServiceProvider()
    {
        serviceProvider ??= services.BuildServiceProvider();
        return serviceProvider;
    }

    public IRawEventProcessingFlow<TPayload> BuildFlow()
    {
        return BuildServiceProvider().GetRequiredService<IRawEventProcessingFlow<TPayload>>();
    }
}
