using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

public sealed class RawEventProcessingFlowBuilder<TPayload> : IRawEventProcessingFlowBuilder<TPayload>
{
    private readonly HandlingPipelineBuilder<TPayload> handlingPipelineBuilder = new();
    private IEnvelopeMaterializer? materializer;

    public IRawEventProcessingFlowBuilder<TPayload> UseMaterializer(IEnvelopeMaterializer materializer)
    {
        this.materializer = materializer ?? throw new ArgumentNullException(nameof(materializer));
        return this;
    }

    public IRawEventProcessingFlowBuilder<TPayload> UseInfrastructureStep(IInfrastructureStep<TPayload> infrastructureMiddleware)
    {
        handlingPipelineBuilder.UseInfrastructureStep(infrastructureMiddleware);
        return this;
    }

    public IRawEventProcessingFlowBuilder<TPayload> UseBusinessStep(IBusinessStep<TPayload> businessMiddleware)
    {
        handlingPipelineBuilder.UseBusinessStep(businessMiddleware);
        return this;
    }

    public IRawEventProcessingFlowBuilder<TPayload> UseHandler(IEventConsumer<TPayload> consumer)
    {
        handlingPipelineBuilder.UseHandler(consumer);
        return this;
    }

    public IRawEventProcessingFlow<TPayload> Build()
    {
        var flowMaterializer = materializer ?? new EnvelopeMaterializer();
        var handlingPipeline = handlingPipelineBuilder.Build();

        return new RawEventProcessingFlow<TPayload>(flowMaterializer, handlingPipeline);
    }
}