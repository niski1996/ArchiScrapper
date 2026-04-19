using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ArchiScrapper.CommonMessaging.ConsumerSimulationTests;

internal sealed class ConsumerSimulationScenarioBuilder<TPayload>
{
    private readonly ConsumerSimulationTestHost<TPayload> host = new();

    public ConsumerSimulationScenarioBuilder<TPayload> WithSingleton<TService>(TService instance)
        where TService : class
    {
        host.AddSingleton(instance);
        return this;
    }

    public ConsumerSimulationScenarioBuilder<TPayload> WithInfrastructureStep<TStep>()
        where TStep : class, IInfrastructureStep<TPayload>
    {
        host.AddInfrastructureStep<TStep>();
        return this;
    }

    public ConsumerSimulationScenarioBuilder<TPayload> WithBusinessStep<TStep>()
        where TStep : class, IBusinessStep<TPayload>
    {
        host.AddBusinessStep<TStep>();
        return this;
    }

    public ConsumerSimulationScenarioBuilder<TPayload> WithConsumer<TConsumer>()
        where TConsumer : class, IEventConsumer<TPayload>
    {
        host.AddConsumer<TConsumer>();
        return this;
    }

    public IServiceProvider BuildServiceProvider()
    {
        return host.BuildServiceProvider();
    }

    public IRawEventProcessingFlow<TPayload> BuildFlow()
    {
        return host.BuildFlow();
    }

    public string SeedPayload<TPayloadRecord>(TPayloadRecord payload)
    {
        return JsonSerializer.Serialize(payload);
    }

    public void SeedStoragePayload(string reference, string payload)
    {
        var storage = (InMemoryPayloadStorageProvider)BuildServiceProvider().GetRequiredService<IPayloadStorageProvider>();
        storage.PutPayload(reference, payload);
    }

    public RawEnvelope CreateInlineEnvelope(string firstName, string lastName, string city, string payload)
    {
        return new RawEnvelope(firstName, lastName, city, payload);
    }

    public RawEnvelope CreateReferenceEnvelope(string firstName, string lastName, string city, string payloadReference)
    {
        return new RawEnvelope(firstName, lastName, city, string.Empty, payloadReference);
    }
}
