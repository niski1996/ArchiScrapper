namespace ArchiScrapper.Messaging.Abstractions;

public interface IInfrastructureStep<TPayload>
{
    Task ExecuteAsync(HandleContext<TPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken);
}