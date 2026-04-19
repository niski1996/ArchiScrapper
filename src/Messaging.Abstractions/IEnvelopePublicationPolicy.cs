namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Groups reusable publication behavior such as error handling and telemetry.
/// </summary>
/// <typeparam name="TPayload">Payload type being published.</typeparam>
public interface IEnvelopePublicationPolicy<TPayload>
{
    /// <summary>
    /// Gets the error handler used by the publication pipeline.
    /// </summary>
    IEnvelopePublicationErrorHandler<TPayload> ErrorHandler { get; }

    /// <summary>
    /// Gets optional telemetry callbacks invoked for publication steps.
    /// </summary>
    IEnvelopePublicationTelemetry<TPayload>? Telemetry { get; }
}