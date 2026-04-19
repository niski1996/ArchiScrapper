namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Decision returned by handling pipeline error handler.
/// </summary>
/// <param name="Action">Action to apply for current error.</param>
public sealed record HandlingPipelineErrorDecision(HandlingPipelineErrorAction Action)
{
    /// <summary>
    /// Gets a decision that retries the failed step.
    /// </summary>
    public static HandlingPipelineErrorDecision Retry { get; } = new(HandlingPipelineErrorAction.Retry);

    /// <summary>
    /// Gets a decision that continues with next step.
    /// </summary>
    public static HandlingPipelineErrorDecision Continue { get; } = new(HandlingPipelineErrorAction.Continue);

    /// <summary>
    /// Gets a decision that stops processing and rethrows.
    /// </summary>
    public static HandlingPipelineErrorDecision Stop { get; } = new(HandlingPipelineErrorAction.Stop);
}