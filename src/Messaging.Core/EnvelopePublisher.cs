using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

public sealed class EnvelopePublisher<TPayload> : IEnvelopePublisher<TPayload>
{
    private readonly IEnvelopePublicationPipeline publicationPipeline;

    public EnvelopePublisher(IEnvelopePublicationPipeline publicationPipeline)
    {
        this.publicationPipeline = publicationPipeline ?? throw new ArgumentNullException(nameof(publicationPipeline));
    }

    public RawEnvelope PublishInline(TypedEnvelope<TPayload> source, Func<TPayload, string> payloadSerializer)
    {
        return publicationPipeline.Compose(source, payloadSerializer);
    }

    public RawEnvelope Publish(TypedEnvelope<TPayload> source, Func<TPayload, string> payloadSerializer)
    {
        return PublishInline(source, payloadSerializer);
    }

    public RawEnvelope PublishWithReference(
        TypedEnvelope<TPayload> source,
        Func<TPayload, string> payloadSerializer,
        string payloadReference)
    {
        return publicationPipeline.ComposeWithReference(source, payloadSerializer, payloadReference);
    }
}