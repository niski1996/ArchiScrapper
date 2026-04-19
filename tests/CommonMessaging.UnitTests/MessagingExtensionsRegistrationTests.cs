using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Extensions;
using ArchiScrapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class MessagingExtensionsRegistrationTests
{
    [Fact]
    public async Task AddCommonMessagingCoreAndFlowRegistersExecutableFlow()
    {
        var services = new ServiceCollection();

        services
            .AddCommonMessagingCore()
            .AddRawEventProcessingFlow<PersonPayload>();

        services.AddSingleton<IEventConsumer<PersonPayload>, SpyConsumer>();

        var serviceProvider = services.BuildServiceProvider();
        var flow = serviceProvider.GetRequiredService<IRawEventProcessingFlow<PersonPayload>>();

        var source = new RawEnvelope(
            "Jan",
            "Kowalski",
            "Krakow",
            JsonSerializer.Serialize(new PersonPayload("Jan", "Kowalski", "Krakow")));

        await flow.ProcessAsync(source, payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);

        var consumer = (SpyConsumer)serviceProvider.GetRequiredService<IEventConsumer<PersonPayload>>();
        Assert.True(consumer.Handled);
    }

    [Fact]
    public void AddCommonMessagingCoreRegistersEnvelopePublisher()
    {
        var services = new ServiceCollection();

        services.AddCommonMessagingCore();

        var serviceProvider = services.BuildServiceProvider();
        var publisher = serviceProvider.GetRequiredService<IEnvelopePublisher<PersonPayload>>();

        var result = publisher.Publish(
            new TypedEnvelope<PersonPayload>(
                "Jan",
                "Kowalski",
                "Krakow",
                new PersonPayload("Jan", "Kowalski", "Krakow")),
            payload => System.Text.Json.JsonSerializer.Serialize(payload));

        Assert.Equal("Jan", result.FirstName);
        Assert.Equal("Kowalski", result.LastName);
        Assert.Equal("Krakow", result.City);
        Assert.Equal("{\"FirstName\":\"Jan\",\"LastName\":\"Kowalski\",\"City\":\"Krakow\"}", result.Payload);
    }

    [Fact]
    public void AddCommonMessagingCoreRegistersReferencePublicationPath()
    {
        var services = new ServiceCollection();

        services.AddCommonMessagingCore();

        var serviceProvider = services.BuildServiceProvider();
        var publisher = serviceProvider.GetRequiredService<IEnvelopePublisher<PersonPayload>>();

        var result = publisher.PublishWithReference(
            new TypedEnvelope<PersonPayload>(
                "Jan",
                "Kowalski",
                "Krakow",
                new PersonPayload("Jan", "Kowalski", "Krakow")),
            payload => System.Text.Json.JsonSerializer.Serialize(payload),
            "payload-1");

        var storage = (InMemoryPayloadStorageProvider)serviceProvider.GetRequiredService<IPayloadStorageProvider>();

        Assert.Equal("payload-1", result.PayloadReference);
        Assert.Equal("{\"FirstName\":\"Jan\",\"LastName\":\"Kowalski\",\"City\":\"Krakow\"}", storage.GetPayload("payload-1"));
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);

    private sealed class SpyConsumer : IEventConsumer<PersonPayload>
    {
        public bool Handled { get; private set; }

        public Task HandleAsync(HandleContext<PersonPayload> context, CancellationToken cancellationToken)
        {
            Handled = true;
            return Task.CompletedTask;
        }
    }
}
