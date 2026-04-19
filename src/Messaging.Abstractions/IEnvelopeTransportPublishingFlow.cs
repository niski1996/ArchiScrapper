using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Publishes typed envelopes by composing raw envelopes and forwarding them to transport publisher.
/// </summary>
/// <typeparam name="TPayload">Payload type being published.</typeparam>
public interface IEnvelopeTransportPublishingFlow<TPayload>
{
    /// <summary>
    /// Publishes using inline payload mode.
    /// </summary>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload into transport string.</param>
    /// <param name="policy">Optional reusable publication policy.</param>
    /// <param name="cancellationToken">Token used to cancel publishing.</param>
    Task PublishInlineAsync(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload>? policy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes using payload-reference mode.
    /// </summary>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload into transport string.</param>
    /// <param name="payloadReference">Reference written to the composed envelope.</param>
    /// <param name="policy">Optional reusable publication policy.</param>
    /// <param name="cancellationToken">Token used to cancel publishing.</param>
    Task PublishWithReferenceAsync(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationPolicy<TPayload>? policy = null,
        CancellationToken cancellationToken = default);
}