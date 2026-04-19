using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Runtime context passed across handling pipeline steps and the final consumer.
/// </summary>
/// <typeparam name="TPayload">Payload type contained by the typed envelope.</typeparam>
public sealed class HandleContext<TPayload>
{
    /// <summary>
    /// Initializes a new handling context.
    /// </summary>
    /// <param name="envelope">Typed envelope being processed.</param>
    public HandleContext(TypedEnvelope<TPayload> envelope)
    {
        Envelope = envelope ?? throw new ArgumentNullException(nameof(envelope));
        Items = new Dictionary<string, object?>();
    }

    /// <summary>
    /// Gets the typed envelope being processed.
    /// </summary>
    public TypedEnvelope<TPayload> Envelope { get; }

    /// <summary>
    /// Gets mutable per-message state shared by pipeline steps.
    /// </summary>
    public IDictionary<string, object?> Items { get; }
}