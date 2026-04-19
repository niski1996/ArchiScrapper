namespace ArchiScrapper.Messaging.Abstractions;

public enum EnvelopePublicationErrorAction
{
    Retry = 0,
    Continue = 1,
    Stop = 2,
}