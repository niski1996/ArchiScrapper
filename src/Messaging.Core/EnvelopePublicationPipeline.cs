using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

public sealed class EnvelopePublicationPipeline : IEnvelopePublicationPipeline
{
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
}