using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public sealed class HandleContext<TPayload>
{
    public HandleContext(TypedEnvelope<TPayload> envelope)
    {
        Envelope = envelope ?? throw new ArgumentNullException(nameof(envelope));
        Items = new Dictionary<string, object?>();
    }

    public TypedEnvelope<TPayload> Envelope { get; }

    public IDictionary<string, object?> Items { get; }
}