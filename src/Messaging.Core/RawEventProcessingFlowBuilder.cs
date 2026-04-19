using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

/// <summary>
/// Default builder for <see cref="IRawEventProcessingFlow{TPayload}"/>.
/// </summary>
/// <typeparam name="TPayload">Target payload type.</typeparam>
public sealed class RawEventProcessingFlowBuilder<TPayload> : IRawEventProcessingFlowBuilder<TPayload>
{
    private readonly HandlingPipelineBuilder<TPayload> handlingPipelineBuilder = new();
    private IEnvelopeMaterializer? materializer;

    /// <inheritdoc />
    public IRawEventProcessingFlowBuilder<TPayload> UseMaterializer(IEnvelopeMaterializer materializer)
    {
        this.materializer = materializer ?? throw new ArgumentNullException(nameof(materializer));
        return this;
    }

    /// <inheritdoc />
    public IRawEventProcessingFlowBuilder<TPayload> UseInfrastructureStep(IInfrastructureStep<TPayload> infrastructureMiddleware)
    {
        handlingPipelineBuilder.UseInfrastructureStep(infrastructureMiddleware);
        return this;
    }

    /// <inheritdoc />
    public IRawEventProcessingFlowBuilder<TPayload> UseBusinessStep(IBusinessStep<TPayload> businessMiddleware)
    {
        handlingPipelineBuilder.UseBusinessStep(businessMiddleware);
        return this;
    }

    /// <inheritdoc />
    public IRawEventProcessingFlowBuilder<TPayload> UseHandler(IEventConsumer<TPayload> consumer)
    {
        handlingPipelineBuilder.UseHandler(consumer);
        return this;
    }

    /// <inheritdoc />
    public IRawEventProcessingFlowBuilder<TPayload> UseErrorHandler(IHandlingPipelineErrorHandler<TPayload> errorHandler)
    {
        handlingPipelineBuilder.UseErrorHandler(errorHandler);
        return this;
    }

    /// <inheritdoc />
    public IRawEventProcessingFlow<TPayload> Build()
    {
        var flowMaterializer = materializer ?? new EnvelopeMaterializer();
        var handlingPipeline = handlingPipelineBuilder.Build();

        return new RawEventProcessingFlow<TPayload>(flowMaterializer, handlingPipeline);
    }
}