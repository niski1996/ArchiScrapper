namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Defines an infrastructure-owned handling step executed before business steps.
/// </summary>
/// <typeparam name="TPayload">Payload type processed by the step.</typeparam>
public interface IInfrastructureStep<TPayload>
{
    /// <summary>
    /// Executes the step and optionally continues pipeline execution.
    /// </summary>
    /// <param name="context">Handling context for the current message.</param>
    /// <param name="continuation">Delegate that continues execution to the next step.</param>
    /// <param name="cancellationToken">Token used to cancel execution.</param>
    Task ExecuteAsync(HandleContext<TPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken);
}