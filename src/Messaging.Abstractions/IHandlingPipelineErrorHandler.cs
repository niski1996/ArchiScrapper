namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Handles errors raised during handling pipeline execution.
/// </summary>
/// <typeparam name="TPayload">Payload type processed by the handling pipeline.</typeparam>
public interface IHandlingPipelineErrorHandler<TPayload>
{
    /// <summary>
    /// Produces a decision for a handling pipeline failure.
    /// </summary>
    /// <param name="context">Failure context describing step, attempt, and exception.</param>
    /// <returns>Decision that controls retry/continue/stop behavior.</returns>
    Task<HandlingPipelineErrorDecision> HandleAsync(HandlingPipelineErrorContext<TPayload> context);
}