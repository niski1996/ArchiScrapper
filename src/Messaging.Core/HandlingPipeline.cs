using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

public sealed class HandlingPipeline<TPayload> : IHandlingPipeline<TPayload>
{
    private readonly IReadOnlyList<IInfrastructureStep<TPayload>> infrastructureSteps;
    private readonly IReadOnlyList<IBusinessStep<TPayload>> businessSteps;
    private readonly IEventConsumer<TPayload> consumer;

    public HandlingPipeline(
        IReadOnlyList<IInfrastructureStep<TPayload>> infrastructureSteps,
        IReadOnlyList<IBusinessStep<TPayload>> businessSteps,
        IEventConsumer<TPayload> consumer)
    {
        this.infrastructureSteps = infrastructureSteps ?? throw new ArgumentNullException(nameof(infrastructureSteps));
        this.businessSteps = businessSteps ?? throw new ArgumentNullException(nameof(businessSteps));
        this.consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
    }

    public Task ExecuteAsync(HandleContext<TPayload> context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        return ExecuteInfrastructureStepAsync(0, context, cancellationToken);
    }

    private Task ExecuteInfrastructureStepAsync(int index, HandleContext<TPayload> context, CancellationToken cancellationToken)
    {
        if (index >= infrastructureSteps.Count)
        {
            return ExecuteBusinessStepAsync(0, context, cancellationToken);
        }

        return infrastructureSteps[index].ExecuteAsync(
            context,
            nextCancellationToken => ExecuteInfrastructureStepAsync(index + 1, context, nextCancellationToken),
            cancellationToken);
    }

    private Task ExecuteBusinessStepAsync(int index, HandleContext<TPayload> context, CancellationToken cancellationToken)
    {
        if (index >= businessSteps.Count)
        {
            return consumer.HandleAsync(context, cancellationToken);
        }

        return businessSteps[index].ExecuteAsync(
            context,
            nextCancellationToken => ExecuteBusinessStepAsync(index + 1, context, nextCancellationToken),
            cancellationToken);
    }
}