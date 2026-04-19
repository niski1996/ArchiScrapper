using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

/// <summary>
/// Default builder for <see cref="IEnvelopePublicationPolicy{TPayload}"/>.
/// </summary>
/// <typeparam name="TPayload">Payload type being published.</typeparam>
public sealed class EnvelopePublicationPolicyBuilder<TPayload> : IEnvelopePublicationPolicyBuilder<TPayload>
{
    private IEnvelopePublicationErrorHandler<TPayload>? errorHandler;
    private IEnvelopePublicationTelemetry<TPayload>? telemetry;

    /// <inheritdoc />
    public IEnvelopePublicationPolicyBuilder<TPayload> UseErrorHandler(IEnvelopePublicationErrorHandler<TPayload> errorHandler)
    {
        this.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        return this;
    }

    /// <inheritdoc />
    public IEnvelopePublicationPolicyBuilder<TPayload> UseTelemetry(IEnvelopePublicationTelemetry<TPayload> telemetry)
    {
        this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        return this;
    }

    /// <inheritdoc />
    public IEnvelopePublicationPolicy<TPayload> Build()
    {
        return new EnvelopePublicationPolicy<TPayload>(
            errorHandler ?? EnvelopePublicationDefaults.StopOnErrorHandler<TPayload>(),
            telemetry ?? EnvelopePublicationDefaults.NoOpTelemetry<TPayload>());
    }

    private sealed record EnvelopePublicationPolicy<TPolicyPayload>(
        IEnvelopePublicationErrorHandler<TPolicyPayload> ErrorHandler,
        IEnvelopePublicationTelemetry<TPolicyPayload>? Telemetry) : IEnvelopePublicationPolicy<TPolicyPayload>;

}