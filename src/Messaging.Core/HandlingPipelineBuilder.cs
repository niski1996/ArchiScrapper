using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

/// <summary>
/// Default builder for <see cref="HandlingPipeline{TPayload}"/>.
/// </summary>
/// <typeparam name="TPayload">Payload type processed by the pipeline.</typeparam>
public sealed class HandlingPipelineBuilder<TPayload> : IHandlingPipelineBuilder<TPayload>
{
    private readonly List<IInfrastructureStep<TPayload>> infrastructureSteps = [];
    private readonly List<IBusinessStep<TPayload>> businessSteps = [];
    private IEventConsumer<TPayload>? consumer;
    private IHandlingPipelineErrorHandler<TPayload>? errorHandler;

    /// <inheritdoc />
    public IHandlingPipelineBuilder<TPayload> UseInfrastructureStep(IInfrastructureStep<TPayload> infrastructureMiddleware)
    {
        ArgumentNullException.ThrowIfNull(infrastructureMiddleware);
        infrastructureSteps.Add(infrastructureMiddleware);
        return this;
    }

    /// <inheritdoc />
    public IHandlingPipelineBuilder<TPayload> UseBusinessStep(IBusinessStep<TPayload> businessMiddleware)
    {
        ArgumentNullException.ThrowIfNull(businessMiddleware);
        businessSteps.Add(businessMiddleware);
        return this;
    }

    /// <inheritdoc />
    public IHandlingPipelineBuilder<TPayload> UseHandler(IEventConsumer<TPayload> consumer)
    {
        this.consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
        return this;
    }

    /// <inheritdoc />
    public IHandlingPipelineBuilder<TPayload> UseErrorHandler(IHandlingPipelineErrorHandler<TPayload> errorHandler)
    {
        this.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        return this;
    }

    /// <inheritdoc />
    public IHandlingPipeline<TPayload> Build()
    {
        if (consumer is null)
        {
            throw new InvalidOperationException("Handling pipeline requires a final handler.");
        }

        return new HandlingPipeline<TPayload>(
            infrastructureSteps.ToArray(),
            businessSteps.ToArray(),
            consumer,
            errorHandler);
    }
}