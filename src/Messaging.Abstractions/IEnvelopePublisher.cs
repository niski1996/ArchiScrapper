using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopePublisher<TPayload>
{
    RawEnvelope Publish(TypedEnvelope<TPayload> source, Func<TPayload, string> payloadSerializer);

    RawEnvelope PublishWithReference(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference);
}