using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

public sealed class EnvelopeMaterializationPipeline : IEnvelopeMaterializationPipeline
{
    private readonly IPayloadSourceResolver payloadSourceResolver;
    private readonly IReadOnlyList<IEnvelopeMaterializationStage> stages;

    public EnvelopeMaterializationPipeline()
        : this(
            new PayloadSourceResolver(new InMemoryPayloadStorageProvider()),
            [
                new ValidateSourceStage(),
                new SelectPayloadStage(),
                new BuildTypedEnvelopeStage(),
            ])
    {
    }

    public EnvelopeMaterializationPipeline(IPayloadSourceResolver payloadSourceResolver)
        : this(
            payloadSourceResolver,
            [
                new ValidateSourceStage(),
                new SelectPayloadStage(),
                new BuildTypedEnvelopeStage(),
            ])
    {
    }

    public EnvelopeMaterializationPipeline(
        IPayloadSourceResolver payloadSourceResolver,
        IEnumerable<IEnvelopeMaterializationStage> stages)
    {
        this.payloadSourceResolver = payloadSourceResolver ?? throw new ArgumentNullException(nameof(payloadSourceResolver));
        ArgumentNullException.ThrowIfNull(stages);
        this.stages = stages.ToArray();
    }

    public TypedEnvelope<TPayload> Materialize<TPayload>(RawEnvelope source, Func<string, TPayload> payloadFactory)
    {
        var context = new EnvelopeMaterializationContext<TPayload>(source, payloadFactory, payloadSourceResolver);

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
            context.RawPayload = context.PayloadSourceResolver.ResolvePayload(context.Source);
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
