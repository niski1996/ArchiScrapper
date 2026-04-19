namespace ArchiScrapper.Messaging.Abstractions;

public sealed record EnvelopePublicationErrorDecision(EnvelopePublicationErrorAction Action)
{
    public static EnvelopePublicationErrorDecision Retry { get; } = new(EnvelopePublicationErrorAction.Retry);

    public static EnvelopePublicationErrorDecision Continue { get; } = new(EnvelopePublicationErrorAction.Continue);

    public static EnvelopePublicationErrorDecision Stop { get; } = new(EnvelopePublicationErrorAction.Stop);
}