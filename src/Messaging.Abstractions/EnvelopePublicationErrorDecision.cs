namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Decision returned by publication error handler.
/// </summary>
/// <param name="Action">Action to apply for current error.</param>
public sealed record EnvelopePublicationErrorDecision(EnvelopePublicationErrorAction Action)
{
    /// <summary>
    /// Gets a decision that retries the failed step.
    /// </summary>
    public static EnvelopePublicationErrorDecision Retry { get; } = new(EnvelopePublicationErrorAction.Retry);

    /// <summary>
    /// Gets a decision that continues with fallback behavior.
    /// </summary>
    public static EnvelopePublicationErrorDecision Continue { get; } = new(EnvelopePublicationErrorAction.Continue);

    /// <summary>
    /// Gets a decision that stops processing and rethrows.
    /// </summary>
    public static EnvelopePublicationErrorDecision Stop { get; } = new(EnvelopePublicationErrorAction.Stop);
}