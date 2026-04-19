using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

public sealed class RawEventProcessingFlow<TPayload> : IRawEventProcessingFlow<TPayload>
{
    private readonly IEnvelopeMaterializer materializer;
    private readonly IHandlingPipeline<TPayload> handlingPipeline;

    public RawEventProcessingFlow(
        IEnvelopeMaterializer materializer,
        IHandlingPipeline<TPayload> handlingPipeline)
    {
        this.materializer = materializer ?? throw new ArgumentNullException(nameof(materializer));
        this.handlingPipeline = handlingPipeline ?? throw new ArgumentNullException(nameof(handlingPipeline));
    }

    public async Task ProcessAsync(
        Models.RawEnvelope source,
        Func<string, TPayload> payloadFactory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(payloadFactory);

        var typedEnvelope = materializer.Materialize(source, payloadFactory);
        var context = new HandleContext<TPayload>(typedEnvelope);

        await handlingPipeline.ExecuteAsync(context, cancellationToken);
    }
}