using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

public sealed class EnvelopePublicationPipeline : IEnvelopePublicationPipeline
{
    private readonly IPayloadStorageWriter payloadStorageWriter;

    public EnvelopePublicationPipeline()
        : this(new InMemoryPayloadStorageProvider())
    {
    }

    public EnvelopePublicationPipeline(IPayloadStorageWriter payloadStorageWriter)
    {
        this.payloadStorageWriter = payloadStorageWriter ?? throw new ArgumentNullException(nameof(payloadStorageWriter));
    }

    public RawEnvelope Compose<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(payloadSerializer);

        var payload = ExecuteWithErrorPolicy(
            source,
            EnvelopePublicationStepKind.Serialize,
            errorHandler,
            () => payloadSerializer(source.Payload) ?? throw new InvalidOperationException("Payload serializer returned null."),
            _ => string.Empty);

        return new RawEnvelope(
            source.FirstName,
            source.LastName,
            source.City,
            payload);
    }

    public RawEnvelope ComposeWithReference<TPayload>(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(payloadSerializer);

        var payload = ExecuteWithErrorPolicy(
            source,
            EnvelopePublicationStepKind.Serialize,
            errorHandler,
            () => payloadSerializer(source.Payload) ?? throw new InvalidOperationException("Payload serializer returned null."),
            _ => string.Empty);

        var stored = ExecuteWithErrorPolicy(
            source,
            EnvelopePublicationStepKind.Store,
            errorHandler,
            () =>
            {
                payloadStorageWriter.PutPayload(payloadReference, payload);
                return true;
            },
            _ => false);

        if (!stored)
        {
            return new RawEnvelope(
                source.FirstName,
                source.LastName,
                source.City,
                payload);
        }

        return new RawEnvelope(
            source.FirstName,
            source.LastName,
            source.City,
            string.Empty,
            payloadReference);
    }

    private static TResult ExecuteWithErrorPolicy<TPayload, TResult>(
        TypedEnvelope<TPayload> source,
        EnvelopePublicationStepKind stepKind,
        IEnvelopePublicationErrorHandler<TPayload>? errorHandler,
        Func<TResult> action,
        Func<EnvelopePublicationErrorContext<TPayload>, TResult> continueValueFactory)
    {
        var attempt = 1;
        var handler = errorHandler ?? new ThrowingEnvelopePublicationErrorHandler<TPayload>();

        while (true)
        {
            try
            {
                return action();
            }
            catch (Exception exception) when (!IsMarkedForRethrow(exception))
            {
                var decision = handler.HandleAsync(
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
                    return continueValueFactory(new EnvelopePublicationErrorContext<TPayload>(
                        source,
                        stepKind,
                        attempt,
                        exception,
                        CancellationToken.None));
                }

                MarkForRethrow(exception);
                throw;
            }
        }
    }

    private static readonly object RethrowMarker = new();

    private static bool IsMarkedForRethrow(Exception exception)
    {
        return exception.Data.Contains(RethrowMarker);
    }

    private static void MarkForRethrow(Exception exception)
    {
        exception.Data[RethrowMarker] = true;
    }

    private sealed class ThrowingEnvelopePublicationErrorHandler<TPipelinePayload> : IEnvelopePublicationErrorHandler<TPipelinePayload>
    {
        public Task<EnvelopePublicationErrorDecision> HandleAsync(EnvelopePublicationErrorContext<TPipelinePayload> context)
        {
            return Task.FromResult(EnvelopePublicationErrorDecision.Stop);
        }
    }
}