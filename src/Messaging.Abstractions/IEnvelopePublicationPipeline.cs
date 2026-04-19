using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Composes <see cref="RawEnvelope"/> instances from typed envelopes using publication policies.
/// </summary>
public interface IEnvelopePublicationPipeline
{
    /// <summary>
    /// Composes a raw envelope with inline payload.
    /// </summary>
    /// <typeparam name="TPayload">Type of the source payload.</typeparam>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload into a transport string.</param>
    /// <param name="errorHandler">Optional call-scoped publication error handler.</param>
    /// <returns>Composed raw envelope.</returns>
    RawEnvelope Compose<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null);

    /// <summary>
    /// Composes a raw envelope with inline payload using a reusable policy.
    /// </summary>
    /// <typeparam name="TPayload">Type of the source payload.</typeparam>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload into a transport string.</param>
    /// <param name="policy">Reusable publication policy.</param>
    /// <returns>Composed raw envelope.</returns>
    RawEnvelope ComposeWithPolicy<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload> policy);

    /// <summary>
    /// Composes a raw envelope that carries a payload reference.
    /// </summary>
    /// <typeparam name="TPayload">Type of the source payload.</typeparam>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload into a transport string.</param>
    /// <param name="payloadReference">Payload reference written to the composed envelope.</param>
    /// <param name="errorHandler">Optional call-scoped publication error handler.</param>
    /// <returns>Composed raw envelope.</returns>
    RawEnvelope ComposeWithReference<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null);

    /// <summary>
    /// Composes a raw envelope that carries a payload reference using a reusable policy.
    /// </summary>
    /// <typeparam name="TPayload">Type of the source payload.</typeparam>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload into a transport string.</param>
    /// <param name="payloadReference">Payload reference written to the composed envelope.</param>
    /// <param name="policy">Reusable publication policy.</param>
    /// <returns>Composed raw envelope.</returns>
    RawEnvelope ComposeWithReferenceWithPolicy<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationPolicy<TPayload> policy);
}