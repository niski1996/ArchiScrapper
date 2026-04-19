namespace ArchiScrapper.Messaging.Abstractions;

public sealed record HandlingPipelineErrorContext<TPayload>(
    HandleContext<TPayload> Context,
    HandlingPipelineStepKind StepKind,
    int StepIndex,
    int Attempt,
    Exception Exception,
    CancellationToken CancellationToken);