using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class EnvelopePublisherTests
{
    [Fact]
    public void PublishDelegatesToPublicationPipeline()
    {
        var publicationPipeline = new StubPublicationPipeline();
        var publisher = new EnvelopePublisher<PersonPayload>(publicationPipeline);
        var source = new TypedEnvelope<PersonPayload>(
            "Jan",
            "Kowalski",
            "Krakow",
            new PersonPayload("Jan", "Kowalski", "Krakow"));

        var result = publisher.PublishInline(source, payload => payload.FirstName);

        Assert.True(publicationPipeline.WasCalled);
        Assert.Equal("composed:Jan", result.Payload);
    }

    [Fact]
    public void PublishWithReferenceDelegatesToPublicationPipeline()
    {
        var publicationPipeline = new StubPublicationPipeline();
        var publisher = new EnvelopePublisher<PersonPayload>(publicationPipeline);
        var source = new TypedEnvelope<PersonPayload>(
            "Jan",
            "Kowalski",
            "Krakow",
            new PersonPayload("Jan", "Kowalski", "Krakow"));

        var result = publisher.PublishWithReference(source, payload => payload.FirstName, "payload-1");

        Assert.True(publicationPipeline.WasCalled);
        Assert.Equal("payload-1", result.PayloadReference);
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);

    private sealed class StubPublicationPipeline : IEnvelopePublicationPipeline
    {
        public bool WasCalled { get; private set; }

        public RawEnvelope Compose<TPayload>(
            TypedEnvelope<TPayload> source,
            Func<TPayload, string> payloadSerializer,
            IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
        {
            WasCalled = true;

            return new RawEnvelope(source.FirstName, source.LastName, source.City, $"composed:{payloadSerializer(source.Payload)}");
        }

        public RawEnvelope ComposeWithReference<TPayload>(
            TypedEnvelope<TPayload> source,
            Func<TPayload, string> payloadSerializer,
            string payloadReference,
            IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
        {
            WasCalled = true;

            return new RawEnvelope(source.FirstName, source.LastName, source.City, string.Empty, payloadReference);
        }
    }
}