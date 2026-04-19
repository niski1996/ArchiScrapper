namespace ArchiScrapper.Messaging.Abstractions;

public interface IEventConsumer<TPayload>
{
    Task HandleAsync(HandleContext<TPayload> context, CancellationToken cancellationToken);
}