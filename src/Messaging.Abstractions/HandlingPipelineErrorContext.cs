namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Context describing one handling pipeline step failure.
/// </summary>
/// <typeparam name="TPayload">Payload type being processed.</typeparam>
public sealed record HandlingPipelineErrorContext<TPayload>(
    HandleContext<TPayload> Context,
    HandlingPipelineStepKind StepKind,
    int StepIndex,
    int Attempt,
    Exception Exception,
    CancellationToken CancellationToken);