using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public sealed record EnvelopePublicationErrorContext<TPayload>(
    TypedEnvelope<TPayload> Source,
    EnvelopePublicationStepKind StepKind,
    int Attempt,
    Exception Exception,
    CancellationToken CancellationToken);