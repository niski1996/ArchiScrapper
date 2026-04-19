using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

public sealed class DefaultEnvelopePublicationErrorHandler<TPayload> : IEnvelopePublicationErrorHandler<TPayload>
{
    private readonly int maxRetriesPerStep;
    private readonly bool continueOnStoreFailure;

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