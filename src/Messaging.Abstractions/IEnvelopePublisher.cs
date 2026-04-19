using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Provides a publish-side facade that composes <see cref="RawEnvelope"/> instances from typed envelopes.
/// </summary>
/// <typeparam name="TPayload">Type of the typed payload.</typeparam>
public interface IEnvelopePublisher<TPayload>
{
    /// <summary>
    /// Publishes a typed envelope using inline payload mode.
    /// </summary>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload to transport string representation.</param>
    /// <param name="errorHandler">Optional call-scoped publication error handler.</param>
    /// <returns>Composed raw envelope with inline payload.</returns>
    RawEnvelope PublishInline(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null);

    /// <summary>
    /// Publishes a typed envelope using inline payload mode and a reusable policy.
    /// </summary>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload to transport string representation.</param>
    /// <param name="policy">Reusable publication policy.</param>
    /// <returns>Composed raw envelope with inline payload.</returns>
    RawEnvelope PublishInlineWithPolicy(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload> policy);

    /// <summary>
    /// Publishes a typed envelope using default publish mode (inline in current implementation).
    /// </summary>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload to transport string representation.</param>
    /// <param name="errorHandler">Optional call-scoped publication error handler.</param>
    /// <returns>Composed raw envelope.</returns>
    RawEnvelope Publish(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null);

    /// <summary>
    /// Publishes a typed envelope using default publish mode with a reusable policy.
    /// </summary>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload to transport string representation.</param>
    /// <param name="policy">Reusable publication policy.</param>
    /// <returns>Composed raw envelope.</returns>
    RawEnvelope PublishWithPolicy(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload> policy);

    /// <summary>
    /// Publishes a typed envelope using payload-reference mode.
    /// </summary>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload to transport string representation.</param>
    /// <param name="payloadReference">External payload reference to place in the composed envelope.</param>
    /// <param name="errorHandler">Optional call-scoped publication error handler.</param>
    /// <returns>Composed raw envelope with payload reference.</returns>
    RawEnvelope PublishWithReference(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null);

    /// <summary>
    /// Publishes a typed envelope using payload-reference mode and a reusable policy.
    /// </summary>
    /// <param name="source">Source typed envelope.</param>
    /// <param name="payloadSerializer">Serializer that converts payload to transport string representation.</param>
    /// <param name="payloadReference">External payload reference to place in the composed envelope.</param>
    /// <param name="policy">Reusable publication policy.</param>
    /// <returns>Composed raw envelope with payload reference.</returns>
    RawEnvelope PublishWithReferenceWithPolicy(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationPolicy<TPayload> policy);
}