using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

public sealed class EnvelopeTransportPublishingFlow<TPayload> : IEnvelopeTransportPublishingFlow<TPayload>
{
    private readonly IEnvelopePublisher<TPayload> envelopePublisher;
    private readonly IRawEnvelopeTransportPublisher transportPublisher;

    public EnvelopeTransportPublishingFlow(
        IEnvelopePublisher<TPayload> envelopePublisher,
        IRawEnvelopeTransportPublisher transportPublisher)
    {
        this.envelopePublisher = envelopePublisher ?? throw new ArgumentNullException(nameof(envelopePublisher));
        this.transportPublisher = transportPublisher ?? throw new ArgumentNullException(nameof(transportPublisher));
    }

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