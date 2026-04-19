namespace ArchiScrapper.Messaging.Abstractions;

public interface IBusinessStep<TPayload>
{
    Task ExecuteAsync(HandleContext<TPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken);
}