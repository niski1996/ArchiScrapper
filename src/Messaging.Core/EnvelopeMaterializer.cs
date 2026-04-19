using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

/// <summary>
/// Thin facade over <see cref="IEnvelopeMaterializationPipeline"/>.
/// </summary>
public sealed class EnvelopeMaterializer : IEnvelopeMaterializer
{
    private readonly IEnvelopeMaterializationPipeline pipeline;

    /// <summary>
    /// Initializes a materializer with default materialization pipeline.
    /// </summary>
    public EnvelopeMaterializer()
        : this(new EnvelopeMaterializationPipeline())
    {
    }

    /// <summary>
    /// Initializes a materializer with provided materialization pipeline.
    /// </summary>
    /// <param name="pipeline">Pipeline used to convert raw envelopes to typed envelopes.</param>
    public EnvelopeMaterializer(IEnvelopeMaterializationPipeline pipeline)
    {
        this.pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    /// <inheritdoc />
    public TypedEnvelope<TPayload> Materialize<TPayload>(RawEnvelope source, Func<string, TPayload> payloadFactory)
    {
        return pipeline.Materialize(source, payloadFactory);
    }
}
