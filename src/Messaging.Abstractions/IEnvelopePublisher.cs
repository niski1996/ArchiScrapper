using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopePublisher<TPayload>
{
    RawEnvelope PublishInline(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null);

    RawEnvelope Publish(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null);

    RawEnvelope PublishWithReference(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null);
}