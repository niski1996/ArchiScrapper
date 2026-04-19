using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Defines a stage-based pipeline for converting raw envelopes into typed envelopes.
/// </summary>
public interface IEnvelopeMaterializationPipeline
{
    /// <summary>
    /// Materializes a typed envelope from raw input using a payload factory.
    /// </summary>
    /// <typeparam name="TPayload">Target payload type.</typeparam>
    /// <param name="source">Raw envelope source.</param>
    /// <param name="payloadFactory">Factory that maps resolved payload text into a typed payload.</param>
    /// <returns>Typed materialized envelope.</returns>
    TypedEnvelope<TPayload> Materialize<TPayload>(RawEnvelope source, Func<string, TPayload> payloadFactory);
}