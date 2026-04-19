using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Extensions;
using ArchiScrapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArchiScrapper.CommonMessaging.IntegrationTests;

public class EnvelopeTransportPublishingFlowIntegrationTests
{
    [Fact]
    public async Task PublishInlineAsyncComposesAndSendsEnvelope()
    {
        var transport = new RecordingTransportPublisher();
        var services = new ServiceCollection();

        services
            .AddCommonMessagingCore()
            .AddEnvelopeTransportPublishingFlow<PersonPayload>();

        services.AddSingleton<IRawEnvelopeTransportPublisher>(transport);

        var provider = services.BuildServiceProvider();
        var flow = provider.GetRequiredService<IEnvelopeTransportPublishingFlow<PersonPayload>>();

        var typed = new TypedEnvelope<PersonPayload>(
            "Anna",
            "Nowak",
            "Gdansk",
            new PersonPayload("Anna", "Nowak", "Gdansk"));

        await flow.PublishInlineAsync(typed, payload => JsonSerializer.Serialize(payload));

        Assert.NotNull(transport.LastPublished);
        Assert.Equal("Anna", transport.LastPublished!.FirstName);
        Assert.Equal("{\"FirstName\":\"Anna\",\"LastName\":\"Nowak\",\"City\":\"Gdansk\"}", transport.LastPublished.Payload);
        Assert.Null(transport.LastPublished.PayloadReference);
    }

    [Fact]
    public async Task PublishWithReferenceAsyncComposesAndSendsReferenceEnvelope()
    {
        var transport = new RecordingTransportPublisher();
        var services = new ServiceCollection();

        services
            .AddCommonMessagingCore()
            .AddEnvelopeTransportPublishingFlow<PersonPayload>();

        services.AddSingleton<IRawEnvelopeTransportPublisher>(transport);

        var provider = services.BuildServiceProvider();
        var flow = provider.GetRequiredService<IEnvelopeTransportPublishingFlow<PersonPayload>>();

        var typed = new TypedEnvelope<PersonPayload>(
            "Jan",
            "Kowalski",
            "Krakow",
            new PersonPayload("Jan", "Kowalski", "Krakow"));

        await flow.PublishWithReferenceAsync(typed, payload => JsonSerializer.Serialize(payload), "person-ref-1");

        Assert.NotNull(transport.LastPublished);
        Assert.Equal("Jan", transport.LastPublished!.FirstName);
        Assert.Equal("person-ref-1", transport.LastPublished.PayloadReference);
        Assert.Equal(string.Empty, transport.LastPublished.Payload);
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);

    private sealed class RecordingTransportPublisher : IRawEnvelopeTransportPublisher
    {
        public RawEnvelope? LastPublished { get; private set; }

        public Task PublishAsync(RawEnvelope envelope, CancellationToken cancellationToken = default)
        {
            LastPublished = envelope;
            return Task.CompletedTask;
        }
    }
}