namespace ArchiScrapper.Messaging.Abstractions;

public interface IRawEventProcessingFlowBuilder<TPayload>
{
    IRawEventProcessingFlowBuilder<TPayload> UseMaterializer(IEnvelopeMaterializer materializer);

    IRawEventProcessingFlowBuilder<TPayload> UseInfrastructureStep(IInfrastructureStep<TPayload> infrastructureMiddleware);

    IRawEventProcessingFlowBuilder<TPayload> UseBusinessStep(IBusinessStep<TPayload> businessMiddleware);

    IRawEventProcessingFlowBuilder<TPayload> UseHandler(IEventConsumer<TPayload> consumer);

    IRawEventProcessingFlow<TPayload> Build();
}