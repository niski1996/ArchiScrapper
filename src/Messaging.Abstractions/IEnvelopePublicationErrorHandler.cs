namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopePublicationErrorHandler<TPayload>
{
    Task<EnvelopePublicationErrorDecision> HandleAsync(EnvelopePublicationErrorContext<TPayload> context);
}