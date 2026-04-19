using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Telemetry context for a publication step attempt.
/// </summary>
/// <typeparam name="TPayload">Payload type being published.</typeparam>
public sealed record EnvelopePublicationTelemetryContext<TPayload>(
    TypedEnvelope<TPayload> Source,
    EnvelopePublicationStepKind StepKind,
    int Attempt,
    Exception? Exception,
    CancellationToken CancellationToken);