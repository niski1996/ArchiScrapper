using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

public sealed class EnvelopeMaterializationPipeline : IEnvelopeMaterializationPipeline
{
    private static readonly IReadOnlyList<IEnvelopeMaterializationStage> DefaultStages =
    [
        new ValidateSourceStage(),
        new SelectPayloadStage(),
        new BuildTypedEnvelopeStage(),
    ];

    private readonly IReadOnlyList<IEnvelopeMaterializationStage> stages;

    public EnvelopeMaterializationPipeline()
        : this(DefaultStages)
    {
    }

    public EnvelopeMaterializationPipeline(IEnumerable<IEnvelopeMaterializationStage> stages)
    {
        ArgumentNullException.ThrowIfNull(stages);
        this.stages = stages.ToArray();
    }

    public TypedEnvelope<TPayload> Materialize<TPayload>(ResolvingExampleEvent source, Func<string, TPayload> payloadFactory)
    {
        var context = new EnvelopeMaterializationContext<TPayload>(source, payloadFactory);

        foreach (var stage in stages)
        {
            stage.Execute(context);
        }

        return context.Result ?? throw new InvalidOperationException("Envelope materialization pipeline did not produce a result.");
    }

    private sealed class ValidateSourceStage : IEnvelopeMaterializationStage
    {
        public void Execute<TPayload>(EnvelopeMaterializationContext<TPayload> context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (string.IsNullOrWhiteSpace(context.Source.FirstName))
            {
                throw new InvalidOperationException("FirstName is required.");
            }

            if (string.IsNullOrWhiteSpace(context.Source.LastName))
            {
                throw new InvalidOperationException("LastName is required.");
            }

            if (string.IsNullOrWhiteSpace(context.Source.City))
            {
                throw new InvalidOperationException("City is required.");
            }
        }
    }

    private sealed class SelectPayloadStage : IEnvelopeMaterializationStage
    {
        public void Execute<TPayload>(EnvelopeMaterializationContext<TPayload> context)
        {
            ArgumentNullException.ThrowIfNull(context);
            context.RawPayload = context.Source.Payload;
            context.Payload = context.PayloadFactory(context.RawPayload);
        }
    }

    private sealed class BuildTypedEnvelopeStage : IEnvelopeMaterializationStage
    {
        public void Execute<TPayload>(EnvelopeMaterializationContext<TPayload> context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Payload is null)
            {
                throw new InvalidOperationException("Payload was not materialized.");
            }

            context.Result = new TypedEnvelope<TPayload>(
                context.Source.FirstName,
                context.Source.LastName,
                context.Source.City,
                context.Payload);
        }
    }
}
