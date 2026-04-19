using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopePublicationPipeline
{
    RawEnvelope Compose<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null);

    RawEnvelope ComposeWithPolicy<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload> policy);

    RawEnvelope ComposeWithReference<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null);

    RawEnvelope ComposeWithReferenceWithPolicy<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationPolicy<TPayload> policy);
}