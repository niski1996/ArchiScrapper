namespace ArchiScrapper.Messaging.Abstractions;

public sealed record HandlingPipelineErrorDecision(HandlingPipelineErrorAction Action)
{
    public static HandlingPipelineErrorDecision Retry { get; } = new(HandlingPipelineErrorAction.Retry);

    public static HandlingPipelineErrorDecision Continue { get; } = new(HandlingPipelineErrorAction.Continue);

    public static HandlingPipelineErrorDecision Stop { get; } = new(HandlingPipelineErrorAction.Stop);
}