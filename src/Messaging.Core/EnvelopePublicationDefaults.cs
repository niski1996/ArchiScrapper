using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

internal static class EnvelopePublicationDefaults
{
    public static IEnvelopePublicationErrorHandler<TPayload> StopOnErrorHandler<TPayload>()
    {
        return new StopOnErrorEnvelopePublicationErrorHandler<TPayload>();
    }

    public static IEnvelopePublicationTelemetry<TPayload> NoOpTelemetry<TPayload>()
    {
        return new NoOpEnvelopePublicationTelemetry<TPayload>();
    }

    private sealed class StopOnErrorEnvelopePublicationErrorHandler<TPayload> : IEnvelopePublicationErrorHandler<TPayload>
    {
        public Task<EnvelopePublicationErrorDecision> HandleAsync(EnvelopePublicationErrorContext<TPayload> context)
        {
            return Task.FromResult(EnvelopePublicationErrorDecision.Stop);
        }
    }

    private sealed class NoOpEnvelopePublicationTelemetry<TPayload> : IEnvelopePublicationTelemetry<TPayload>
    {
        public void OnStepStarting(EnvelopePublicationTelemetryContext<TPayload> context)
        {
        }

        public void OnStepSucceeded(EnvelopePublicationTelemetryContext<TPayload> context)
        {
        }

        public void OnStepFailed(EnvelopePublicationTelemetryContext<TPayload> context)
        {
        }
    }
}