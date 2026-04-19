using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Context describing one publication step failure.
/// </summary>
/// <typeparam name="TPayload">Payload type being published.</typeparam>
public sealed record EnvelopePublicationErrorContext<TPayload>(
    TypedEnvelope<TPayload> Source,
    EnvelopePublicationStepKind StepKind,
    int Attempt,
    Exception Exception,
    CancellationToken CancellationToken);