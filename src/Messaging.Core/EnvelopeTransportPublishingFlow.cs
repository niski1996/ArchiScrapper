using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

/// <summary>
/// Composes raw envelopes from typed inputs and forwards them to the transport publisher.
/// </summary>
/// <typeparam name="TPayload">Payload type being published.</typeparam>
public sealed class EnvelopeTransportPublishingFlow<TPayload> : IEnvelopeTransportPublishingFlow<TPayload>
{
    private readonly IEnvelopePublisher<TPayload> envelopePublisher;
    private readonly IRawEnvelopeTransportPublisher transportPublisher;

    /// <summary>
    /// Initializes a new instance of transport publishing flow.
    /// </summary>
    /// <param name="envelopePublisher">Typed envelope publisher facade.</param>
    /// <param name="transportPublisher">Transport publisher used for raw envelope handoff.</param>
    public EnvelopeTransportPublishingFlow(
        IEnvelopePublisher<TPayload> envelopePublisher,
        IRawEnvelopeTransportPublisher transportPublisher)
    {
        this.envelopePublisher = envelopePublisher ?? throw new ArgumentNullException(nameof(envelopePublisher));
        this.transportPublisher = transportPublisher ?? throw new ArgumentNullException(nameof(transportPublisher));
    }

    /// <inheritdoc />
    public Task PublishInlineAsync(
        Models.TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        IEnvelopePublicationPolicy<TPayload>? policy = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(payloadSerializer);

        var raw = policy is null
            ? envelopePublisher.PublishInline(source, payloadSerializer)
            : envelopePublisher.PublishInlineWithPolicy(source, payloadSerializer, policy);

        return transportPublisher.PublishAsync(raw, cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishWithReferenceAsync(
        Models.TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference,
        IEnvelopePublicationPolicy<TPayload>? policy = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(payloadSerializer);

        var raw = policy is null
            ? envelopePublisher.PublishWithReference(source, payloadSerializer, payloadReference)
            : envelopePublisher.PublishWithReferenceWithPolicy(source, payloadSerializer, payloadReference, policy);

        return transportPublisher.PublishAsync(raw, cancellationToken);
    }
}