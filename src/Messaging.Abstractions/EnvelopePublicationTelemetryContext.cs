using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public sealed record EnvelopePublicationTelemetryContext<TPayload>(
    TypedEnvelope<TPayload> Source,
    EnvelopePublicationStepKind StepKind,
    int Attempt,
    Exception? Exception,
    CancellationToken CancellationToken);