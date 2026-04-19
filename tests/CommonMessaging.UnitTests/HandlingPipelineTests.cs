using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class HandlingPipelineTests
{
    [Fact]
    public async Task ExecuteAsyncRunsInfrastructureThenBusinessThenHandler()
    {
        var calls = new List<string>();

        var pipeline = new HandlingPipelineBuilder<PersonPayload>()
            .UseInfrastructureStep(new TrackingInfrastructureStep("infra-1", calls))
            .UseInfrastructureStep(new TrackingInfrastructureStep("infra-2", calls))
            .UseBusinessStep(new TrackingBusinessStep("business-1", calls))
            .UseBusinessStep(new TrackingBusinessStep("business-2", calls))
            .UseHandler(new TrackingHandler(calls))
            .Build();

        var context = new HandleContext<PersonPayload>(
            new TypedEnvelope<PersonPayload>(
                "Jan",
                "Kowalski",
                "Krakow",
                new PersonPayload("P1")));

        await pipeline.ExecuteAsync(context);

        Assert.Collection(
            calls,
            item => Assert.Equal("infra-1:before", item),
            item => Assert.Equal("infra-2:before", item),
            item => Assert.Equal("business-1:before", item),
            item => Assert.Equal("business-2:before", item),
            item => Assert.Equal("handler", item),
            item => Assert.Equal("business-2:after", item),
            item => Assert.Equal("business-1:after", item),
            item => Assert.Equal("infra-2:after", item),
            item => Assert.Equal("infra-1:after", item));
    }

    [Fact]
    public void BuildThrowsWhenHandlerWasNotConfigured()
    {
        var builder = new HandlingPipelineBuilder<PersonPayload>()
            .UseInfrastructureStep(new TrackingInfrastructureStep("infra", []));

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    private sealed record PersonPayload(string Value);

    private sealed class TrackingInfrastructureStep(string name, List<string> calls) : IInfrastructureStep<PersonPayload>
    {
        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add($"{name}:before");
            await continuation(cancellationToken);
            calls.Add($"{name}:after");
        }
    }

    private sealed class TrackingBusinessStep(string name, List<string> calls) : IBusinessStep<PersonPayload>
    {
        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add($"{name}:before");
            await continuation(cancellationToken);
            calls.Add($"{name}:after");
        }
    }

    private sealed class TrackingHandler(List<string> calls) : IEventConsumer<PersonPayload>
    {
        public Task HandleAsync(HandleContext<PersonPayload> context, CancellationToken cancellationToken)
        {
            calls.Add("handler");
            return Task.CompletedTask;
        }
    }
}