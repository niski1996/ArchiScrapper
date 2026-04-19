using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Resolves payload text from either inline envelope data or external payload reference.
/// </summary>
public interface IPayloadSourceResolver
{
    /// <summary>
    /// Resolves payload text for a raw envelope.
    /// </summary>
    /// <param name="source">Raw envelope containing inline payload or payload reference.</param>
    /// <returns>Resolved payload text.</returns>
    string ResolvePayload(RawEnvelope source);
}