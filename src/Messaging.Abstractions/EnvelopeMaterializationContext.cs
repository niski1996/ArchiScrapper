using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Mutable context shared across materialization pipeline stages.
/// </summary>
/// <typeparam name="TPayload">Target payload type.</typeparam>
public sealed class EnvelopeMaterializationContext<TPayload>
{
    /// <summary>
    /// Initializes a new materialization context.
    /// </summary>
    /// <param name="source">Raw source envelope.</param>
    /// <param name="payloadFactory">Factory that maps payload text to typed payload.</param>
    /// <param name="payloadSourceResolver">Resolver that selects payload source.</param>
    public EnvelopeMaterializationContext(
        RawEnvelope source,
        Func<string, TPayload> payloadFactory,
        IPayloadSourceResolver payloadSourceResolver)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        PayloadFactory = payloadFactory ?? throw new ArgumentNullException(nameof(payloadFactory));
        PayloadSourceResolver = payloadSourceResolver ?? throw new ArgumentNullException(nameof(payloadSourceResolver));
    }

    /// <summary>
    /// Gets the raw source envelope.
    /// </summary>
    public RawEnvelope Source { get; }

    /// <summary>
    /// Gets the payload factory.
    /// </summary>
    public Func<string, TPayload> PayloadFactory { get; }

    /// <summary>
    /// Gets the payload source resolver.
    /// </summary>
    public IPayloadSourceResolver PayloadSourceResolver { get; }

    /// <summary>
    /// Gets or sets raw payload text resolved from source.
    /// </summary>
    public string? RawPayload { get; set; }

    /// <summary>
    /// Gets or sets typed payload produced by <see cref="PayloadFactory"/>.
    /// </summary>
    public TPayload? Payload { get; set; }

    /// <summary>
    /// Gets or sets final materialization result.
    /// </summary>
    public TypedEnvelope<TPayload>? Result { get; set; }
}