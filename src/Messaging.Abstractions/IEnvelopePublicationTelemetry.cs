namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopePublicationTelemetry<TPayload>
{
    void OnStepStarting(EnvelopePublicationTelemetryContext<TPayload> context);

    void OnStepSucceeded(EnvelopePublicationTelemetryContext<TPayload> context);

    void OnStepFailed(EnvelopePublicationTelemetryContext<TPayload> context);
}