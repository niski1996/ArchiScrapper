namespace ArchiScrapper.Messaging.Abstractions;

public interface IHandlingPipelineBuilder<TPayload>
{
    IHandlingPipelineBuilder<TPayload> UseInfrastructureStep(IInfrastructureStep<TPayload> infrastructureMiddleware);

    IHandlingPipelineBuilder<TPayload> UseBusinessStep(IBusinessStep<TPayload> businessMiddleware);

    IHandlingPipelineBuilder<TPayload> UseHandler(IEventConsumer<TPayload> consumer);

    IHandlingPipelineBuilder<TPayload> UseErrorHandler(IHandlingPipelineErrorHandler<TPayload> errorHandler);

    IHandlingPipeline<TPayload> Build();
}