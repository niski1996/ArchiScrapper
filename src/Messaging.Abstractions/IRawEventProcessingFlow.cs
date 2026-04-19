using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Executes the full consume-side flow from raw envelope to handling pipeline.
/// </summary>
/// <typeparam name="TPayload">Target payload type.</typeparam>
public interface IRawEventProcessingFlow<TPayload>
{
    /// <summary>
    /// Processes one raw envelope through materialization and handling.
    /// </summary>
    /// <param name="source">Raw envelope source.</param>
    /// <param name="payloadFactory">Factory that materializes the typed payload from payload text.</param>
    /// <param name="cancellationToken">Token used to cancel processing.</param>
    Task ProcessAsync(
    RawEnvelope source,
        Func<string, TPayload> payloadFactory,
        CancellationToken cancellationToken = default);
}