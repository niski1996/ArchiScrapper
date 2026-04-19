using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopeMaterializationPipeline
{
    TypedEnvelope<TPayload> Materialize<TPayload>(ResolvingExampleEvent source, Func<string, TPayload> payloadFactory);
}