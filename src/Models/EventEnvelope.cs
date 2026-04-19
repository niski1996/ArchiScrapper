using ArchiScrapper.Contracts;

namespace ArchiScrapper.Models;

public sealed record EventEnvelope : IEvent, IHasPayload, IHasPayloadReference
{
    public EventEnvelope(
        Guid id,
        string eventType,
        DateTimeOffset creationDate,
        long timestamp,
        string payload,
        string? payloadReference = null)
    {
        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(eventType));
        }

        ArgumentNullException.ThrowIfNull(payload);

        Id = id;
        EventType = eventType;
        CreationDate = creationDate;
        Timestamp = timestamp;
        Payload = payload;
        PayloadReference = payloadReference;
    }

    public Guid Id { get; }

    public string EventType { get; }

    public DateTimeOffset CreationDate { get; }

    public long Timestamp { get; }

    public string Payload { get; }

    public string? PayloadReference { get; }

    public static EventEnvelope Create(
        string eventType,
        string payload,
        string? payloadReference = null)
    {
        var now = DateTimeOffset.UtcNow;

        return new EventEnvelope(
            Guid.NewGuid(),
            eventType,
            now,
            now.ToUnixTimeMilliseconds(),
            payload,
            payloadReference);
    }
}
