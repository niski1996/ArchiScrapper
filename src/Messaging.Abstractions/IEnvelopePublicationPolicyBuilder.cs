namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Builds reusable publication policies.
/// </summary>
/// <typeparam name="TPayload">Payload type being published.</typeparam>
public interface IEnvelopePublicationPolicyBuilder<TPayload>
{
    /// <summary>
    /// Sets the error handler for the policy.
    /// </summary>
    /// <param name="errorHandler">Publication error handler instance.</param>
    /// <returns>Current builder instance.</returns>
    IEnvelopePublicationPolicyBuilder<TPayload> UseErrorHandler(IEnvelopePublicationErrorHandler<TPayload> errorHandler);

    /// <summary>
    /// Sets telemetry callbacks for the policy.
    /// </summary>
    /// <param name="telemetry">Publication telemetry implementation.</param>
    /// <returns>Current builder instance.</returns>
    IEnvelopePublicationPolicyBuilder<TPayload> UseTelemetry(IEnvelopePublicationTelemetry<TPayload> telemetry);

    /// <summary>
    /// Builds the configured publication policy.
    /// </summary>
    /// <returns>Configured policy instance.</returns>
    IEnvelopePublicationPolicy<TPayload> Build();
}