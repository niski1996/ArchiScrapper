namespace ArchiScrapper.Messaging.Abstractions;

public interface IEnvelopeMaterializationStage
{
    void Execute<TPayload>(EnvelopeMaterializationContext<TPayload> context);
}