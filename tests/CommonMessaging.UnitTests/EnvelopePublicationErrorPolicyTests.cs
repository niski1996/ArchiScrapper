using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class EnvelopePublicationErrorPolicyTests
{
    [Fact]
    public void ComposeInlineRetriesUntilSerializerSucceeds()
    {
        var attempts = 0;
        var pipeline = new EnvelopePublicationPipeline();
        var source = CreateSource();

        var result = pipeline.Compose(
            source,
            _ =>
            {
                attempts++;

                if (attempts == 1)
                {
                    throw new InvalidOperationException("Transient serializer failure.");
                }

                return "payload-ok";
            },
            new SequencedErrorHandler(EnvelopePublicationErrorDecision.Retry, EnvelopePublicationErrorDecision.Stop));

        Assert.Equal("payload-ok", result.Payload);
        Assert.Equal(2, attempts);
    }

    [Fact]
    public void ComposeInlineContinuesWithEmptyPayloadWhenPolicyRequestsContinue()
    {
        var pipeline = new EnvelopePublicationPipeline();
        var source = CreateSource();

        var result = pipeline.Compose(
            source,
            _ => throw new InvalidOperationException("Serializer failed."),
            new ConstantDecisionErrorHandler(EnvelopePublicationErrorDecision.Continue));

        Assert.Equal(string.Empty, result.Payload);
        Assert.Null(result.PayloadReference);
    }

    [Fact]
    public void ComposeWithReferenceFallsBackToInlineWhenStorageFailsAndPolicyContinues()
    {
        var storage = new ThrowingStorageWriter();
        var pipeline = new EnvelopePublicationPipeline(storage);
        var source = CreateSource();

        var result = pipeline.ComposeWithReference(
            source,
            payload => payload.Value,
            "payload-1",
            new ConstantDecisionErrorHandler(EnvelopePublicationErrorDecision.Continue));

        Assert.Equal("serialized", result.Payload);
        Assert.Null(result.PayloadReference);
    }

    [Fact]
    public void ComposeStopsAndRethrowsWhenPolicyRequestsStop()
    {
        var pipeline = new EnvelopePublicationPipeline();
        var source = CreateSource();

        Assert.Throws<InvalidOperationException>(() => pipeline.Compose(
            source,
            _ => throw new InvalidOperationException("Serializer failed."),
            new ConstantDecisionErrorHandler(EnvelopePublicationErrorDecision.Stop)));
    }

    private static TypedEnvelope<PersonPayload> CreateSource()
    {
        return new TypedEnvelope<PersonPayload>(
            "Jan",
            "Kowalski",
            "Krakow",
            new PersonPayload("serialized"));
    }

    private sealed record PersonPayload(string Value);

    private sealed class SequencedErrorHandler(params EnvelopePublicationErrorDecision[] decisions) : IEnvelopePublicationErrorHandler<PersonPayload>
    {
        private readonly Queue<EnvelopePublicationErrorDecision> decisions = new(decisions);

        public Task<EnvelopePublicationErrorDecision> HandleAsync(EnvelopePublicationErrorContext<PersonPayload> context)
        {
            if (decisions.Count == 0)
            {
                return Task.FromResult(EnvelopePublicationErrorDecision.Stop);
            }

            return Task.FromResult(decisions.Dequeue());
        }
    }

    private sealed class ConstantDecisionErrorHandler(EnvelopePublicationErrorDecision decision) : IEnvelopePublicationErrorHandler<PersonPayload>
    {
        public Task<EnvelopePublicationErrorDecision> HandleAsync(EnvelopePublicationErrorContext<PersonPayload> context)
        {
            return Task.FromResult(decision);
        }
    }

    private sealed class ThrowingStorageWriter : IPayloadStorageWriter
    {
        public void PutPayload(string payloadReference, string payload)
        {
            throw new InvalidOperationException("Storage failed.");
        }
    }
}