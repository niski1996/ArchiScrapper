namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Action selected by publication error policy when a publication step fails.
/// </summary>
public enum EnvelopePublicationErrorAction
{
    /// <summary>
    /// Retry the same step.
    /// </summary>
    Retry = 0,

    /// <summary>
    /// Continue flow with step-specific fallback behavior.
    /// </summary>
    Continue = 1,

    /// <summary>
    /// Stop processing and rethrow error.
    /// </summary>
    Stop = 2,
}