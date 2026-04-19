namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Receives callbacks for publish-side pipeline step lifecycle events.
/// </summary>
/// <typeparam name="TPayload">Payload type being published.</typeparam>
public interface IEnvelopePublicationTelemetry<TPayload>
{
    /// <summary>
    /// Called when a publication step starts.
    /// </summary>
    /// <param name="context">Telemetry context for the current step.</param>
    void OnStepStarting(EnvelopePublicationTelemetryContext<TPayload> context);

    /// <summary>
    /// Called when a publication step succeeds.
    /// </summary>
    /// <param name="context">Telemetry context for the current step.</param>
    void OnStepSucceeded(EnvelopePublicationTelemetryContext<TPayload> context);

    /// <summary>
    /// Called when a publication step fails.
    /// </summary>
    /// <param name="context">Telemetry context for the current step.</param>
    void OnStepFailed(EnvelopePublicationTelemetryContext<TPayload> context);
}