namespace ArchiScrapper.Messaging.Abstractions;

public interface IHandlingPipelineErrorHandler<TPayload>
{
    Task<HandlingPipelineErrorDecision> HandleAsync(HandlingPipelineErrorContext<TPayload> context);
}