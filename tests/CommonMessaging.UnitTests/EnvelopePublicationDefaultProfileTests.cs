using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Messaging.Extensions;
using ArchiScrapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class EnvelopePublicationDefaultProfileTests
{
    [Fact]
    public void DefaultProfileRetriesSerializationAndSucceeds()
    {
        var services = new ServiceCollection();
        services.AddCommonMessagingCore();

        var provider = services.BuildServiceProvider();
        var publisher = provider.GetRequiredService<IEnvelopePublisher<PersonPayload>>();
        var builder = provider.GetRequiredService<IEnvelopePublicationPolicyBuilder<PersonPayload>>();

        var policy = builder.UseDefaultProfile(maxRetriesPerStep: 1).Build();
        var source = CreateEnvelope();

        var attempts = 0;
        var raw = publisher.PublishInlineWithPolicy(
            source,
            _ =>
            {
                attempts++;

                if (attempts == 1)
                {
                    throw new InvalidOperationException("Transient serialization failure.");
                }

                return "payload-ok";
            },
            policy);

        Assert.Equal(2, attempts);
        Assert.Equal("payload-ok", raw.Payload);
    }

    [Fact]
    public void DefaultProfileCanFallbackToInlineWhenStorageFails()
    {
        var services = new ServiceCollection();
        services.AddCommonMessagingCore();
        services.AddSingleton<IPayloadStorageProvider, ThrowingStorageProvider>();
        services.AddSingleton<IPayloadStorageWriter>(sp =>
            (IPayloadStorageWriter)sp.GetRequiredService<IPayloadStorageProvider>());

        var provider = services.BuildServiceProvider();
        var publisher = provider.GetRequiredService<IEnvelopePublisher<PersonPayload>>();
        var builder = provider.GetRequiredService<IEnvelopePublicationPolicyBuilder<PersonPayload>>();

        var policy = builder
            .UseDefaultProfile(maxRetriesPerStep: 0, continueOnStoreFailure: true)
            .Build();

        var raw = publisher.PublishWithReferenceWithPolicy(
            CreateEnvelope(),
            payload => payload.FirstName,
            "person-ref-1",
            policy);

        Assert.Equal("Jan", raw.Payload);
        Assert.Null(raw.PayloadReference);
    }

    private static TypedEnvelope<PersonPayload> CreateEnvelope()
    {
        return new TypedEnvelope<PersonPayload>(
            "Jan",
            "Kowalski",
            "Krakow",
            new PersonPayload("Jan", "Kowalski", "Krakow"));
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);

    private sealed class ThrowingStorageProvider : IPayloadStorageProvider, IPayloadStorageWriter
    {
        public string GetPayload(string payloadReference)
        {
            throw new NotSupportedException();
        }

        public void PutPayload(string payloadReference, string payload)
        {
            throw new InvalidOperationException("Storage unavailable.");
        }
    }
}