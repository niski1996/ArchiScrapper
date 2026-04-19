using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class HandlingPipelineErrorPolicyTests
{
    [Fact]
    public async Task ExecuteAsyncRetriesTheCurrentBranchWhenPolicyRequestsRetry()
    {
        var calls = new List<string>();
        var handler = new SequencedErrorHandler(
            HandlingPipelineErrorDecision.Retry,
            HandlingPipelineErrorDecision.Stop);

        var pipeline = new HandlingPipelineBuilder<PersonPayload>()
            .UseErrorHandler(handler)
            .UseInfrastructureStep(new FlakyInfrastructureStep(calls))
            .UseBusinessStep(new RecordingBusinessStep(calls))
            .UseHandler(new RecordingConsumer(calls))
            .Build();

        await pipeline.ExecuteAsync(new HandleContext<PersonPayload>(CreateEnvelope()));

        Assert.Equal(
            [
                "infra:attempt-1",
                "infra:attempt-2",
                "business",
                "consumer",
            ],
            calls);
    }

    [Fact]
    public async Task ExecuteAsyncContinuesWithNextStepWhenPolicyRequestsContinue()
    {
        var calls = new List<string>();

        var pipeline = new HandlingPipelineBuilder<PersonPayload>()
            .UseErrorHandler(new ConstantDecisionErrorHandler(HandlingPipelineErrorDecision.Continue))
            .UseInfrastructureStep(new ThrowingInfrastructureStep(calls))
            .UseInfrastructureStep(new RecordingInfrastructureStep(calls))
            .UseBusinessStep(new RecordingBusinessStep(calls))
            .UseHandler(new RecordingConsumer(calls))
            .Build();

        await pipeline.ExecuteAsync(new HandleContext<PersonPayload>(CreateEnvelope()));

        Assert.Equal(
            [
                "infra:fail",
                "infra:ok",
                "business",
                "consumer",
            ],
            calls);
    }

    [Fact]
    public async Task ExecuteAsyncStopsAndRethrowsWhenPolicyRequestsStop()
    {
        var calls = new List<string>();

        var pipeline = new HandlingPipelineBuilder<PersonPayload>()
            .UseErrorHandler(new ConstantDecisionErrorHandler(HandlingPipelineErrorDecision.Stop))
            .UseInfrastructureStep(new ThrowingInfrastructureStep(calls))
            .UseBusinessStep(new RecordingBusinessStep(calls))
            .UseHandler(new RecordingConsumer(calls))
            .Build();

        await Assert.ThrowsAsync<InvalidOperationException>(() => pipeline.ExecuteAsync(new HandleContext<PersonPayload>(CreateEnvelope())));

        Assert.Equal(["infra:fail"], calls);
    }

    private static TypedEnvelope<PersonPayload> CreateEnvelope()
    {
        return new TypedEnvelope<PersonPayload>(
            "Jan",
            "Kowalski",
            "Krakow",
            new PersonPayload("P1"));
    }

    private sealed record PersonPayload(string Value);

    private sealed class SequencedErrorHandler(params HandlingPipelineErrorDecision[] decisions) : IHandlingPipelineErrorHandler<PersonPayload>
    {
        private readonly Queue<HandlingPipelineErrorDecision> decisions = new(decisions);

        public Task<HandlingPipelineErrorDecision> HandleAsync(HandlingPipelineErrorContext<PersonPayload> context)
        {
            if (decisions.Count == 0)
            {
                return Task.FromResult(HandlingPipelineErrorDecision.Stop);
            }

            return Task.FromResult(decisions.Dequeue());
        }
    }

    private sealed class ConstantDecisionErrorHandler(HandlingPipelineErrorDecision decision) : IHandlingPipelineErrorHandler<PersonPayload>
    {
        public Task<HandlingPipelineErrorDecision> HandleAsync(HandlingPipelineErrorContext<PersonPayload> context)
        {
            return Task.FromResult(decision);
        }
    }

    private sealed class FlakyInfrastructureStep(List<string> calls) : IInfrastructureStep<PersonPayload>
    {
        private int attempts;

        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            attempts++;
            calls.Add($"infra:attempt-{attempts}");

            if (attempts == 1)
            {
                throw new InvalidOperationException("Transient infrastructure failure.");
            }

            await continuation(cancellationToken);
        }
    }

    private sealed class ThrowingInfrastructureStep(List<string> calls) : IInfrastructureStep<PersonPayload>
    {
        public Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add("infra:fail");
            throw new InvalidOperationException("Planned infrastructure failure.");
        }
    }

    private sealed class RecordingInfrastructureStep(List<string> calls) : IInfrastructureStep<PersonPayload>
    {
        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add("infra:ok");
            await continuation(cancellationToken);
        }
    }

    private sealed class RecordingBusinessStep(List<string> calls) : IBusinessStep<PersonPayload>
    {
        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add("business");
            await continuation(cancellationToken);
        }
    }

    private sealed class RecordingConsumer(List<string> calls) : IEventConsumer<PersonPayload>
    {
        public Task HandleAsync(HandleContext<PersonPayload> context, CancellationToken cancellationToken)
        {
            calls.Add("consumer");
            return Task.CompletedTask;
        }
    }
}