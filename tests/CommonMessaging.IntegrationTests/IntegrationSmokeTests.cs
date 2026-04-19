using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.CommonMessaging.IntegrationTests;

public class ProcessingIntegrationTests
{
    [Fact]
    public async Task RawEventFlowProcessesWithInfrastructureAndBusinessSteps()
    {
        var calls = new List<string>();

        var pipeline = new HandlingPipelineBuilder<PersonPayload>()
            .UseInfrastructureStep(new IntegrationInfrastructureStep(calls))
            .UseBusinessStep(new IntegrationBusinessStep(calls))
            .UseHandler(new IntegrationConsumer(calls))
            .Build();

        var flow = new RawEventProcessingFlow<PersonPayload>(new EnvelopeMaterializer(), pipeline);
        var source = new RawEnvelope(
            "Jan",
            "Kowalski",
            "Krakow",
            JsonSerializer.Serialize(new PersonPayload("Jan", "Kowalski", "Krakow")));

        await flow.ProcessAsync(source, payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);

        Assert.Collection(
            calls,
            item => Assert.Equal("infra", item),
            item => Assert.Equal("business", item),
            item => Assert.Equal("consumer:Jan", item));
    }

    [Fact]
    public async Task RawEventFlowFailsWhenTechnicalMetadataIsInvalid()
    {
        var pipeline = new HandlingPipelineBuilder<PersonPayload>()
            .UseHandler(new NoOpConsumer())
            .Build();

        var flow = new RawEventProcessingFlow<PersonPayload>(new EnvelopeMaterializer(), pipeline);
        var source = new RawEnvelope(
            "Jan",
            "Kowalski",
            "",
            JsonSerializer.Serialize(new PersonPayload("Jan", "Kowalski", "Krakow")));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => flow.ProcessAsync(source, payload => JsonSerializer.Deserialize<PersonPayload>(payload)!));
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);

    private sealed class IntegrationInfrastructureStep(List<string> calls) : IInfrastructureStep<PersonPayload>
    {
        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add("infra");
            await continuation(cancellationToken);
        }
    }

    private sealed class IntegrationBusinessStep(List<string> calls) : IBusinessStep<PersonPayload>
    {
        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add("business");
            await continuation(cancellationToken);
        }
    }

    private sealed class IntegrationConsumer(List<string> calls) : IEventConsumer<PersonPayload>
    {
        public Task HandleAsync(HandleContext<PersonPayload> context, CancellationToken cancellationToken)
        {
            calls.Add($"consumer:{context.Envelope.Payload.FirstName}");
            return Task.CompletedTask;
        }
    }

    private sealed class NoOpConsumer : IEventConsumer<PersonPayload>
    {
        public Task HandleAsync(HandleContext<PersonPayload> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
