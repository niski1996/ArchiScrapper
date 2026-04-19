namespace ArchiScrapper.Messaging.Abstractions;

public interface IHandlingPipeline<TPayload>
{
    Task ExecuteAsync(HandleContext<TPayload> context, CancellationToken cancellationToken = default);
}