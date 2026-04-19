using ArchiScrapper.Messaging.Abstractions;

namespace ArchiScrapper.Messaging.Core;

/// <summary>
/// Default orchestration flow from raw envelope materialization to handling pipeline execution.
/// </summary>
/// <typeparam name="TPayload">Target payload type.</typeparam>
public sealed class RawEventProcessingFlow<TPayload> : IRawEventProcessingFlow<TPayload>
{
    private readonly IEnvelopeMaterializer materializer;
    private readonly IHandlingPipeline<TPayload> handlingPipeline;

    /// <summary>
    /// Initializes a new processing flow.
    /// </summary>
    /// <param name="materializer">Materializer used to build typed envelope from raw envelope.</param>
    /// <param name="handlingPipeline">Handling pipeline used to execute registered steps and consumer.</param>
    public RawEventProcessingFlow(
        IEnvelopeMaterializer materializer,
        IHandlingPipeline<TPayload> handlingPipeline)
    {
        this.materializer = materializer ?? throw new ArgumentNullException(nameof(materializer));
        this.handlingPipeline = handlingPipeline ?? throw new ArgumentNullException(nameof(handlingPipeline));
    }

    /// <inheritdoc />
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