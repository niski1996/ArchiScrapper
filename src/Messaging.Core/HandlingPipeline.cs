using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

public sealed class HandlingPipeline<TPayload> : IHandlingPipeline<TPayload>
{
    private readonly IReadOnlyList<IInfrastructureStep<TPayload>> infrastructureSteps;
    private readonly IReadOnlyList<IBusinessStep<TPayload>> businessSteps;
    private readonly IEventConsumer<TPayload> consumer;
    private readonly IHandlingPipelineErrorHandler<TPayload> errorHandler;

    public HandlingPipeline(
        IReadOnlyList<IInfrastructureStep<TPayload>> infrastructureSteps,
        IReadOnlyList<IBusinessStep<TPayload>> businessSteps,
        IEventConsumer<TPayload> consumer,
        IHandlingPipelineErrorHandler<TPayload>? errorHandler = null)
    {
        this.infrastructureSteps = infrastructureSteps ?? throw new ArgumentNullException(nameof(infrastructureSteps));
        this.businessSteps = businessSteps ?? throw new ArgumentNullException(nameof(businessSteps));
        this.consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
        this.errorHandler = errorHandler ?? new ThrowingHandlingPipelineErrorHandler<TPayload>();
    }

    public Task ExecuteAsync(HandleContext<TPayload> context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        return ExecuteInfrastructureStepAsync(0, context, cancellationToken);
    }

    private async Task ExecuteInfrastructureStepAsync(int index, HandleContext<TPayload> context, CancellationToken cancellationToken)
    {
        if (index >= infrastructureSteps.Count)
        {
            await ExecuteBusinessStepAsync(0, context, cancellationToken);
            return;
        }

        await ExecuteWithErrorPolicyAsync(
            context,
            HandlingPipelineStepKind.Infrastructure,
            index,
            nextCancellationToken => ExecuteInfrastructureStepAsync(index + 1, context, nextCancellationToken),
            () => infrastructureSteps[index].ExecuteAsync(
                context,
                nextCancellationToken => ExecuteInfrastructureStepAsync(index + 1, context, nextCancellationToken),
                cancellationToken),
            cancellationToken);
    }

    private async Task ExecuteBusinessStepAsync(int index, HandleContext<TPayload> context, CancellationToken cancellationToken)
    {
        if (index >= businessSteps.Count)
        {
            await ExecuteWithErrorPolicyAsync(
                context,
                HandlingPipelineStepKind.Consumer,
                0,
                _ => Task.CompletedTask,
                () => consumer.HandleAsync(context, cancellationToken),
                cancellationToken);

            return;
        }

        await ExecuteWithErrorPolicyAsync(
            context,
            HandlingPipelineStepKind.Business,
            index,
            nextCancellationToken => ExecuteBusinessStepAsync(index + 1, context, nextCancellationToken),
            () => businessSteps[index].ExecuteAsync(
                context,
                nextCancellationToken => ExecuteBusinessStepAsync(index + 1, context, nextCancellationToken),
                cancellationToken),
            cancellationToken);
    }

    private async Task ExecuteWithErrorPolicyAsync(
        HandleContext<TPayload> context,
        HandlingPipelineStepKind stepKind,
        int stepIndex,
        Func<CancellationToken, Task> continueAction,
        Func<Task> stepAction,
        CancellationToken cancellationToken)
    {
        var attempt = 1;

        while (true)
        {
            try
            {
                await stepAction();
                return;
            }
            catch (Exception exception)
            {
                var decision = await errorHandler.HandleAsync(
                    new HandlingPipelineErrorContext<TPayload>(
                        context,
                        stepKind,
                        stepIndex,
                        attempt,
                        exception,
                        cancellationToken));

                if (decision.Action == HandlingPipelineErrorAction.Retry)
                {
                    attempt++;
                    continue;
                }

                if (decision.Action == HandlingPipelineErrorAction.Continue)
                {
                    await continueAction(cancellationToken);
                    return;
                }

                throw;
            }
        }
    }

    private sealed class ThrowingHandlingPipelineErrorHandler<TPipelinePayload> : IHandlingPipelineErrorHandler<TPipelinePayload>
    {
        public Task<HandlingPipelineErrorDecision> HandleAsync(HandlingPipelineErrorContext<TPipelinePayload> context)
        {
            return Task.FromResult(HandlingPipelineErrorDecision.Stop);
        }
    }
}