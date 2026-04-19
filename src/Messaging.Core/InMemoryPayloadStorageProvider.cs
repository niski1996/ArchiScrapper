using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

public sealed class InMemoryPayloadStorageProvider : IPayloadStorageProvider
    , IPayloadStorageWriter
{
    private readonly Dictionary<string, string> data;

    public InMemoryPayloadStorageProvider(IDictionary<string, string>? seed = null)
    {
        data = new Dictionary<string, string>(seed ?? new Dictionary<string, string>());
    }

    public string GetPayload(string payloadReference)
    {
        if (string.IsNullOrWhiteSpace(payloadReference))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(payloadReference));
        }

        if (!data.TryGetValue(payloadReference, out var payload))
        {
            throw new KeyNotFoundException($"Payload reference '{payloadReference}' was not found.");
        }

        return payload;
    }

    public void PutPayload(string payloadReference, string payload)
    {
        if (string.IsNullOrWhiteSpace(payloadReference))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(payloadReference));
        }

        ArgumentNullException.ThrowIfNull(payload);
        data[payloadReference] = payload;
    }
}