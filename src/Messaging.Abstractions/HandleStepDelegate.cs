namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Continuation delegate used by handling steps to invoke the next step.
/// </summary>
/// <param name="cancellationToken">Token used to cancel continuation.</param>
public delegate Task HandleStepContinuation(CancellationToken cancellationToken);