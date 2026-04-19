using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Publishes raw envelopes to the underlying transport layer.
/// </summary>
public interface IRawEnvelopeTransportPublisher
{
    /// <summary>
    /// Publishes a raw envelope to transport.
    /// </summary>
    /// <param name="envelope">Envelope to publish.</param>
    /// <param name="cancellationToken">Token used to cancel publishing.</param>
    Task PublishAsync(RawEnvelope envelope, CancellationToken cancellationToken = default);
}