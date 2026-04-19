using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

/// <summary>
/// Default publish-side facade delegating envelope composition to <see cref="IEnvelopePublicationPipeline"/>.
/// </summary>
/// <typeparam name="TPayload">Payload type being published.</typeparam>
public sealed class EnvelopePublisher<TPayload> : IEnvelopePublisher<TPayload>
{
    private readonly IEnvelopePublicationPipeline publicationPipeline;

    /// <summary>
    /// Initializes a new instance of the publisher facade.
    /// </summary>
    /// <param name="publicationPipeline">Publication pipeline used to compose raw envelopes.</param>
    public EnvelopePublisher(IEnvelopePublicationPipeline publicationPipeline)
    {
        this.publicationPipeline = publicationPipeline ?? throw new ArgumentNullException(nameof(publicationPipeline));
    }

    /// <inheritdoc />
    public RawEnvelope PublishInline(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
    {
        return publicationPipeline.Compose(source, payloadSerializer, errorHandler);
    }

    /// <inheritdoc />
    public RawEnvelope PublishInlineWithPolicy(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload> policy)
    {
        return publicationPipeline.ComposeWithPolicy(source, payloadSerializer, policy);
    }

    /// <inheritdoc />
    public RawEnvelope Publish(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
    {
        return PublishInline(source, payloadSerializer, errorHandler);
    }

    /// <inheritdoc />
    public RawEnvelope PublishWithPolicy(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload> policy)
    {
        return PublishInlineWithPolicy(source, payloadSerializer, policy);
    }

    /// <inheritdoc />
    public RawEnvelope PublishWithReference(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
    {
        return publicationPipeline.ComposeWithReference(source, payloadSerializer, payloadReference, errorHandler);
    }

    /// <inheritdoc />
    public RawEnvelope PublishWithReferenceWithPolicy(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationPolicy<TPayload> policy)
    {
        return publicationPipeline.ComposeWithReferenceWithPolicy(source, payloadSerializer, payloadReference, policy);
    }
}