using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.CommonMessaging.E2ETests;

public class ConsumerConfiguredPipelineE2ETests
{
    [Fact]
    public async Task ConsumerReceivesTypedPayloadAfterFrameworkPreprocessing()
    {
        var calls = new List<string>();

        var flow = new RawEventProcessingFlowBuilder<PersonPayload>()
            .UseInfrastructureStep(new LoggingInfrastructureStep(calls))
            .UseBusinessStep(new BusinessValidationStep(calls))
            .UseHandler(new BusinessConsumer(calls))
            .Build();

        var source = new ResolvingExampleEvent(
            "Anna",
            "Nowak",
            "Gdansk",
            JsonSerializer.Serialize(new PersonPayload("Anna", "Nowak", "Gdansk")));

        await flow.ProcessAsync(source, payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);

        Assert.Collection(
            calls,
            item => Assert.Equal("framework:log", item),
            item => Assert.Equal("business:validate", item),
            item => Assert.Equal("business:handle:Anna", item));
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);

    private sealed class LoggingInfrastructureStep(List<string> calls) : IInfrastructureStep<PersonPayload>
    {
        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add("framework:log");
            await continuation(cancellationToken);
        }
    }

    private sealed class BusinessValidationStep(List<string> calls) : IBusinessStep<PersonPayload>
    {
        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add("business:validate");
            await continuation(cancellationToken);
        }
    }

    private sealed class BusinessConsumer(List<string> calls) : IEventConsumer<PersonPayload>
    {
        public Task HandleAsync(HandleContext<PersonPayload> context, CancellationToken cancellationToken)
        {
            calls.Add($"business:handle:{context.Envelope.Payload.FirstName}");
            return Task.CompletedTask;
        }
    }
}
