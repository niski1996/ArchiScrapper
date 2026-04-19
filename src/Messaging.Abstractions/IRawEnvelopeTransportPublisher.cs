using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public interface IRawEnvelopeTransportPublisher
{
    Task PublishAsync(RawEnvelope envelope, CancellationToken cancellationToken = default);
}