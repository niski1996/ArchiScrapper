using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Materializes typed envelopes from raw transport envelopes.
/// </summary>
public interface IEnvelopeMaterializer
{
    /// <summary>
    /// Converts a raw envelope to a typed envelope.
    /// </summary>
    /// <typeparam name="TPayload">Target payload type.</typeparam>
    /// <param name="source">Raw source envelope.</param>
    /// <param name="payloadFactory">Factory that converts resolved payload text into a typed payload.</param>
    /// <returns>Materialized typed envelope.</returns>
    TypedEnvelope<TPayload> Materialize<TPayload>(RawEnvelope source, Func<string, TPayload> payloadFactory);
}