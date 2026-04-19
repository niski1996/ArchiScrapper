using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

public sealed class EnvelopePublisher<TPayload> : IEnvelopePublisher<TPayload>
{
    private readonly IEnvelopePublicationPipeline publicationPipeline;

    public EnvelopePublisher(IEnvelopePublicationPipeline publicationPipeline)
    {
        this.publicationPipeline = publicationPipeline ?? throw new ArgumentNullException(nameof(publicationPipeline));
    }

    public RawEnvelope PublishInline(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
    {
        return publicationPipeline.Compose(source, payloadSerializer, errorHandler);
    }

    public RawEnvelope PublishInlineWithPolicy(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload> policy)
    {
        return publicationPipeline.ComposeWithPolicy(source, payloadSerializer, policy);
    }

    public RawEnvelope Publish(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
    {
        return PublishInline(source, payloadSerializer, errorHandler);
    }

    public RawEnvelope PublishWithPolicy(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload> policy)
    {
        return PublishInlineWithPolicy(source, payloadSerializer, policy);
    }

    public RawEnvelope PublishWithReference(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
    {
        return publicationPipeline.ComposeWithReference(source, payloadSerializer, payloadReference, errorHandler);
    }

    public RawEnvelope PublishWithReferenceWithPolicy(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationPolicy<TPayload> policy)
    {
        return publicationPipeline.ComposeWithReferenceWithPolicy(source, payloadSerializer, payloadReference, policy);
    }
}