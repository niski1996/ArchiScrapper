namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopePublicationPolicyBuilder<TPayload>
{
    IEnvelopePublicationPolicyBuilder<TPayload> UseErrorHandler(IEnvelopePublicationErrorHandler<TPayload> errorHandler);

    IEnvelopePublicationPolicyBuilder<TPayload> UseTelemetry(IEnvelopePublicationTelemetry<TPayload> telemetry);

    IEnvelopePublicationPolicy<TPayload> Build();
}