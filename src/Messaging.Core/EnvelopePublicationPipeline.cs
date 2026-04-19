using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

public sealed class EnvelopePublicationPipeline : IEnvelopePublicationPipeline
{
    private readonly IPayloadStorageWriter payloadStorageWriter;

    public EnvelopePublicationPipeline()
        : this(new InMemoryPayloadStorageProvider())
    {
    }

    public EnvelopePublicationPipeline(IPayloadStorageWriter payloadStorageWriter)
    {
        this.payloadStorageWriter = payloadStorageWriter ?? throw new ArgumentNullException(nameof(payloadStorageWriter));
    }

    public RawEnvelope Compose<TPayload>(TypedEnvelope<TPayload> source, Func<TPayload, string> payloadSerializer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(payloadSerializer);

        var payload = payloadSerializer(source.Payload) ?? throw new InvalidOperationException("Payload serializer returned null.");

        return new RawEnvelope(
            source.FirstName,
            source.LastName,
            source.City,
            payload);
    }

    public RawEnvelope ComposeWithReference<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(payloadSerializer);

        var payload = payloadSerializer(source.Payload) ?? throw new InvalidOperationException("Payload serializer returned null.");
        payloadStorageWriter.PutPayload(payloadReference, payload);

        return new RawEnvelope(
            source.FirstName,
            source.LastName,
            source.City,
            string.Empty,
            payloadReference);
    }
}