using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public interface IRawEventProcessingFlow<TPayload>
{
    Task ProcessAsync(
    RawEnvelope source,
        Func<string, TPayload> payloadFactory,
        CancellationToken cancellationToken = default);
}