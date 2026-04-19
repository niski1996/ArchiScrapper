using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopeTransportPublishingFlow<TPayload>
{
    Task PublishInlineAsync(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload>? policy = null,
        CancellationToken cancellationToken = default);

    Task PublishWithReferenceAsync(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationPolicy<TPayload>? policy = null,
        CancellationToken cancellationToken = default);
}