using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

/// <summary>
/// Default publication error handler that supports bounded retries and optional store-failure fallback.
/// </summary>
/// <typeparam name="TPayload">Payload type being published.</typeparam>
public sealed class DefaultEnvelopePublicationErrorHandler<TPayload> : IEnvelopePublicationErrorHandler<TPayload>
{
    private readonly int maxRetriesPerStep;
    private readonly bool continueOnStoreFailure;

    /// <summary>
    /// Initializes a new default publication error handler.
    /// </summary>
    /// <param name="maxRetriesPerStep">Maximum retry attempts for a failed publication step.</param>
    /// <param name="continueOnStoreFailure">Whether store-step failures can continue with fallback behavior.</param>
    public DefaultEnvelopePublicationErrorHandler(
        int maxRetriesPerStep = 1,
        bool continueOnStoreFailure = true)
    {
        if (maxRetriesPerStep < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetriesPerStep), "Max retries cannot be negative.");
        }

        this.maxRetriesPerStep = maxRetriesPerStep;
        this.continueOnStoreFailure = continueOnStoreFailure;
    }

    /// <inheritdoc />
    public Task<EnvelopePublicationErrorDecision> HandleAsync(EnvelopePublicationErrorContext<TPayload> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Attempt <= maxRetriesPerStep)
        {
            return Task.FromResult(EnvelopePublicationErrorDecision.Retry);
        }

        if (continueOnStoreFailure && context.StepKind == EnvelopePublicationStepKind.Store)
        {
            return Task.FromResult(EnvelopePublicationErrorDecision.Continue);
        }

        return Task.FromResult(EnvelopePublicationErrorDecision.Stop);
    }
}