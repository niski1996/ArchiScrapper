namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Executes infrastructure steps, business steps, and final consumer handling for a typed envelope.
/// </summary>
/// <typeparam name="TPayload">Payload type processed by the pipeline.</typeparam>
public interface IHandlingPipeline<TPayload>
{
    /// <summary>
    /// Executes the handling pipeline.
    /// </summary>
    /// <param name="context">Handling context with envelope and per-message state bag.</param>
    /// <param name="cancellationToken">Token used to cancel pipeline execution.</param>
    Task ExecuteAsync(HandleContext<TPayload> context, CancellationToken cancellationToken = default);
}