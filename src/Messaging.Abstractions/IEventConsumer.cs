namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Final typed consumer endpoint in the handling pipeline.
/// </summary>
/// <typeparam name="TPayload">Payload type handled by the consumer.</typeparam>
public interface IEventConsumer<TPayload>
{
    /// <summary>
    /// Handles the materialized envelope context.
    /// </summary>
    /// <param name="context">Handling context for the current message.</param>
    /// <param name="cancellationToken">Token used to cancel handling.</param>
    Task HandleAsync(HandleContext<TPayload> context, CancellationToken cancellationToken);
}