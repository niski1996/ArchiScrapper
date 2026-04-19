namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Defines a business-owned handling step executed after infrastructure steps and before final consumer.
/// </summary>
/// <typeparam name="TPayload">Payload type processed by the step.</typeparam>
public interface IBusinessStep<TPayload>
{
    /// <summary>
    /// Executes the step and optionally continues pipeline execution.
    /// </summary>
    /// <param name="context">Handling context for the current message.</param>
    /// <param name="continuation">Delegate that continues execution to the next step.</param>
    /// <param name="cancellationToken">Token used to cancel execution.</param>
    Task ExecuteAsync(HandleContext<TPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken);
}