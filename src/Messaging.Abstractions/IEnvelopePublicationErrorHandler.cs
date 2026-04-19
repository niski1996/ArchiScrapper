namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Handles errors raised during publish-side envelope composition.
/// </summary>
/// <typeparam name="TPayload">Payload type being published.</typeparam>
public interface IEnvelopePublicationErrorHandler<TPayload>
{
    /// <summary>
    /// Produces a decision for a publication failure.
    /// </summary>
    /// <param name="context">Failure context describing step, attempt, and exception.</param>
    /// <returns>Decision that controls retry/continue/stop behavior.</returns>
    Task<EnvelopePublicationErrorDecision> HandleAsync(EnvelopePublicationErrorContext<TPayload> context);
}