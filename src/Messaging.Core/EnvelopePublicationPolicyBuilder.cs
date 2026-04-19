using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

public sealed class EnvelopePublicationPolicyBuilder<TPayload> : IEnvelopePublicationPolicyBuilder<TPayload>
{
    private IEnvelopePublicationErrorHandler<TPayload>? errorHandler;
    private IEnvelopePublicationTelemetry<TPayload>? telemetry;

    public IEnvelopePublicationPolicyBuilder<TPayload> UseErrorHandler(IEnvelopePublicationErrorHandler<TPayload> errorHandler)
    {
        this.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        return this;
    }

    public IEnvelopePublicationPolicyBuilder<TPayload> UseTelemetry(IEnvelopePublicationTelemetry<TPayload> telemetry)
    {
        this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        return this;
    }

    public IEnvelopePublicationPolicy<TPayload> Build()
    {
        return new EnvelopePublicationPolicy<TPayload>(
            errorHandler ?? new ThrowingEnvelopePublicationErrorHandler<TPayload>(),
            telemetry ?? new NoOpEnvelopePublicationTelemetry<TPayload>());
    }

    private sealed record EnvelopePublicationPolicy<TPolicyPayload>(
        IEnvelopePublicationErrorHandler<TPolicyPayload> ErrorHandler,
        IEnvelopePublicationTelemetry<TPolicyPayload>? Telemetry) : IEnvelopePublicationPolicy<TPolicyPayload>;

    private sealed class NoOpEnvelopePublicationTelemetry<TPolicyPayload> : IEnvelopePublicationTelemetry<TPolicyPayload>
    {
        public void OnStepStarting(EnvelopePublicationTelemetryContext<TPolicyPayload> context)
        {
        }

        public void OnStepSucceeded(EnvelopePublicationTelemetryContext<TPolicyPayload> context)
        {
        }

        public void OnStepFailed(EnvelopePublicationTelemetryContext<TPolicyPayload> context)
        {
        }
    }

    private sealed class ThrowingEnvelopePublicationErrorHandler<TPolicyPayload> : IEnvelopePublicationErrorHandler<TPolicyPayload>
    {
        public Task<EnvelopePublicationErrorDecision> HandleAsync(EnvelopePublicationErrorContext<TPolicyPayload> context)
        {
            return Task.FromResult(EnvelopePublicationErrorDecision.Stop);
        }
    }
}