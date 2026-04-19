using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public sealed class EnvelopeMaterializationContext<TPayload>
{
    public EnvelopeMaterializationContext(ResolvingExampleEvent source, Func<string, TPayload> payloadFactory)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        PayloadFactory = payloadFactory ?? throw new ArgumentNullException(nameof(payloadFactory));
    }

    public ResolvingExampleEvent Source { get; }

    public Func<string, TPayload> PayloadFactory { get; }

    public string? RawPayload { get; set; }

    public TPayload? Payload { get; set; }

    public TypedEnvelope<TPayload>? Result { get; set; }
}