using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopePublicationPipeline
{
    RawEnvelope Compose<TPayload>(TypedEnvelope<TPayload> source, Func<TPayload, string> payloadSerializer);
}