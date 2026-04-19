namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Publish-side step identifiers used by error handling and telemetry.
/// </summary>
public enum EnvelopePublicationStepKind
{
    /// <summary>
    /// Payload serialization step.
    /// </summary>
    Serialize = 0,

    /// <summary>
    /// External payload store write step.
    /// </summary>
    Store = 1,

    /// <summary>
    /// Final envelope composition step.
    /// </summary>
    Compose = 2,
}