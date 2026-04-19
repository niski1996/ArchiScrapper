namespace ArchiScrapper.Messaging.Abstractions;

public enum EnvelopePublicationStepKind
{
    Serialize = 0,
    Store = 1,
    Compose = 2,
}