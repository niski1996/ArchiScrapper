using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

public sealed class EnvelopeMaterializer : IEnvelopeMaterializer
{
    private readonly IEnvelopeMaterializationPipeline pipeline;

    public EnvelopeMaterializer()
        : this(new EnvelopeMaterializationPipeline())
    {
    }

    public EnvelopeMaterializer(IEnvelopeMaterializationPipeline pipeline)
    {
        this.pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    public TypedEnvelope<TPayload> Materialize<TPayload>(ResolvingExampleEvent source, Func<string, TPayload> payloadFactory)
    {
        return pipeline.Materialize(source, payloadFactory);
    }
}
