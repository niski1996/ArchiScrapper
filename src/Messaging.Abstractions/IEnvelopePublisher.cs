using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopePublisher<TPayload>
{
    RawEnvelope PublishInline(TypedEnvelope<TPayload> source, Func<TPayload, string> payloadSerializer);

    RawEnvelope Publish(TypedEnvelope<TPayload> source, Func<TPayload, string> payloadSerializer);

    RawEnvelope PublishWithReference(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference);
}