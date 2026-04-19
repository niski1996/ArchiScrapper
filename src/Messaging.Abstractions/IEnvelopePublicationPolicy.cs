namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopePublicationPolicy<TPayload>
{
    IEnvelopePublicationErrorHandler<TPayload> ErrorHandler { get; }

    IEnvelopePublicationTelemetry<TPayload>? Telemetry { get; }
}