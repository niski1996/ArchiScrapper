using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class RawEventProcessingFlowTests
{
    [Fact]
    public async Task ProcessAsyncMaterializesThenRunsHandlingPipeline()
    {
        var calls = new List<string>();

        var handlingPipeline = new HandlingPipelineBuilder<PersonPayload>()
            .UseInfrastructureStep(new TrackingInfrastructureStep(calls))
            .UseBusinessStep(new TrackingBusinessStep(calls))
            .UseHandler(new TrackingConsumer(calls))
            .Build();

        var flow = new RawEventProcessingFlow<PersonPayload>(
            new EnvelopeMaterializer(),
            handlingPipeline);

        var rawPayload = JsonSerializer.Serialize(new PersonPayload("Jan", "Kowalski", "Krakow"));
        var source = new ResolvingExampleEvent("Jan", "Kowalski", "Krakow", rawPayload);

        await flow.ProcessAsync(
            source,
            payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);

        Assert.Collection(
            calls,
            item => Assert.Equal("infra", item),
            item => Assert.Equal("business", item),
            item => Assert.Equal("consumer:Jan", item));
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);

    private sealed class TrackingInfrastructureStep(List<string> calls) : IInfrastructureStep<PersonPayload>
    {
        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add("infra");
            await continuation(cancellationToken);
        }
    }

    private sealed class TrackingBusinessStep(List<string> calls) : IBusinessStep<PersonPayload>
    {
        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add("business");
            await continuation(cancellationToken);
        }
    }

    private sealed class TrackingConsumer(List<string> calls) : IEventConsumer<PersonPayload>
    {
        public Task HandleAsync(HandleContext<PersonPayload> context, CancellationToken cancellationToken)
        {
            calls.Add($"consumer:{context.Envelope.Payload.FirstName}");
            return Task.CompletedTask;
        }
    }
}