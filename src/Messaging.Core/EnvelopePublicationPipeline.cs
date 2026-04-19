using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

/// <summary>
/// Default publish-side pipeline that serializes payload, optionally stores payload by reference,
/// and composes a <see cref="RawEnvelope"/>.
/// </summary>
public sealed class EnvelopePublicationPipeline : IEnvelopePublicationPipeline
{
    private readonly IPayloadStorageWriter payloadStorageWriter;

    /// <summary>
    /// Initializes a new instance of the publication pipeline with in-memory payload storage writer.
    /// </summary>
    public EnvelopePublicationPipeline()
        : this(new InMemoryPayloadStorageProvider())
    {
    }

    /// <summary>
    /// Initializes a new instance of the publication pipeline.
    /// </summary>
    /// <param name="payloadStorageWriter">Writer used by payload-reference publication path.</param>
    public EnvelopePublicationPipeline(IPayloadStorageWriter payloadStorageWriter)
    {
        this.payloadStorageWriter = payloadStorageWriter ?? throw new ArgumentNullException(nameof(payloadStorageWriter));
    }

    /// <inheritdoc />
    public RawEnvelope Compose<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
    {
        var policy = new EnvelopePublicationPolicy<TPayload>(
            errorHandler ?? EnvelopePublicationDefaults.StopOnErrorHandler<TPayload>(),
            EnvelopePublicationDefaults.NoOpTelemetry<TPayload>());

        return ComposeCore(source, payloadSerializer, policy, payloadReference: null);
    }

    /// <inheritdoc />
    public RawEnvelope ComposeWithPolicy<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload> policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        return ComposeCore(source, payloadSerializer, policy, payloadReference: null);
    }

    /// <inheritdoc />
    public RawEnvelope ComposeWithReference<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
    {
        var policy = new EnvelopePublicationPolicy<TPayload>(
            errorHandler ?? EnvelopePublicationDefaults.StopOnErrorHandler<TPayload>(),
            EnvelopePublicationDefaults.NoOpTelemetry<TPayload>());

        return ComposeCore(source, payloadSerializer, policy, payloadReference);
    }

    /// <inheritdoc />
    public RawEnvelope ComposeWithReferenceWithPolicy<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationPolicy<TPayload> policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        return ComposeCore(source, payloadSerializer, policy, payloadReference);
    }

    private RawEnvelope ComposeCore<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload> policy,
        string? payloadReference)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(payloadSerializer);

        var serializedPayload = ExecuteWithPolicy(
            source,
            EnvelopePublicationStepKind.Serialize,
            policy,
            () => payloadSerializer(source.Payload) ?? throw new InvalidOperationException("Payload serializer returned null."),
            _ => string.Empty);

        if (payloadReference is null)
        {
            return new RawEnvelope(source.FirstName, source.LastName, source.City, serializedPayload);
        }

        var stored = ExecuteWithPolicy(
            source,
            EnvelopePublicationStepKind.Store,
            policy,
            () =>
            {
                payloadStorageWriter.PutPayload(payloadReference, serializedPayload);
                return true;
            },
            _ => false);

        if (!stored)
        {
            return new RawEnvelope(source.FirstName, source.LastName, source.City, serializedPayload);
        }

        return new RawEnvelope(source.FirstName, source.LastName, source.City, string.Empty, payloadReference);
    }

    private static TResult ExecuteWithPolicy<TPayload, TResult>(
        TypedEnvelope<TPayload> source,
        EnvelopePublicationStepKind stepKind,
        IEnvelopePublicationPolicy<TPayload> policy,
        Func<TResult> action,
        Func<EnvelopePublicationTelemetryContext<TPayload>, TResult> continueValueFactory)
    {
        var attempt = 1;

        while (true)
        {
            var telemetryContext = new EnvelopePublicationTelemetryContext<TPayload>(
                source,
                stepKind,
                attempt,
                null,
                CancellationToken.None);

            policy.Telemetry?.OnStepStarting(telemetryContext);

            try
            {
                var result = action();
                policy.Telemetry?.OnStepSucceeded(telemetryContext);
                return result;
            }
            catch (Exception exception)
            {
                policy.Telemetry?.OnStepFailed(telemetryContext with { Exception = exception });

                var decision = policy.ErrorHandler.HandleAsync(
                    new EnvelopePublicationErrorContext<TPayload>(
                        source,
                        stepKind,
                        attempt,
                        exception,
                        CancellationToken.None)).GetAwaiter().GetResult();

                if (decision.Action == EnvelopePublicationErrorAction.Retry)
                {
                    attempt++;
                    continue;
                }

                if (decision.Action == EnvelopePublicationErrorAction.Continue)
                {
                    return continueValueFactory(new EnvelopePublicationTelemetryContext<TPayload>(
                        source,
                        stepKind,
                        attempt,
                        exception,
                        CancellationToken.None));
                }

                throw;
            }
        }
    }

    private sealed record EnvelopePublicationPolicy<TPolicyPayload>(
        IEnvelopePublicationErrorHandler<TPolicyPayload> ErrorHandler,
        IEnvelopePublicationTelemetry<TPolicyPayload>? Telemetry) : IEnvelopePublicationPolicy<TPolicyPayload>;
}