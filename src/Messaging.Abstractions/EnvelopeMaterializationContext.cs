using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public sealed class EnvelopeMaterializationContext<TPayload>
{
    public EnvelopeMaterializationContext(
        RawEnvelope source,
        Func<string, TPayload> payloadFactory,
        IPayloadSourceResolver payloadSourceResolver)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        PayloadFactory = payloadFactory ?? throw new ArgumentNullException(nameof(payloadFactory));
        PayloadSourceResolver = payloadSourceResolver ?? throw new ArgumentNullException(nameof(payloadSourceResolver));
    }

    public RawEnvelope Source { get; }

    public Func<string, TPayload> PayloadFactory { get; }

    public IPayloadSourceResolver PayloadSourceResolver { get; }

    public string? RawPayload { get; set; }

    public TPayload? Payload { get; set; }

    public TypedEnvelope<TPayload>? Result { get; set; }
}