using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class RawEventProcessingFlowBuilderTests
{
    [Fact]
    public async Task BuildCreatesWorkingFlowWithDefaultMaterializer()
    {
        var calls = new List<string>();

        var flow = new RawEventProcessingFlowBuilder<PersonPayload>()
            .UseInfrastructureStep(new RecordingInfrastructureStep(calls))
            .UseBusinessStep(new RecordingBusinessStep(calls))
            .UseHandler(new RecordingConsumer(calls))
            .Build();

        var source = CreateSource("Jan", "Kowalski", "Krakow");

        await flow.ProcessAsync(source, payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);

        Assert.Collection(
            calls,
            item => Assert.Equal("infra", item),
            item => Assert.Equal("business", item),
            item => Assert.Equal("consumer:Jan", item));
    }

    [Fact]
    public async Task BuildUsesProvidedMaterializer()
    {
        var calls = new List<string>();
        var customMaterializer = new StubMaterializer();

        var flow = new RawEventProcessingFlowBuilder<PersonPayload>()
            .UseMaterializer(customMaterializer)
            .UseHandler(new RecordingConsumer(calls))
            .Build();

        var source = CreateSource("Jan", "Kowalski", "Krakow");

        await flow.ProcessAsync(source, payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);

        Assert.True(customMaterializer.WasCalled);
        Assert.Collection(calls, item => Assert.Equal("consumer:Stub", item));
    }

    private static RawEnvelope CreateSource(string firstName, string lastName, string city)
    {
        var payload = JsonSerializer.Serialize(new PersonPayload(firstName, lastName, city));
        return new RawEnvelope(firstName, lastName, city, payload);
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);

    private sealed class RecordingInfrastructureStep(List<string> calls) : IInfrastructureStep<PersonPayload>
    {
        public async Task ExecuteAsync(HandleContext<PersonPayload> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            calls.Add("infra");
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
            calls.Add($"consumer:{context.Envelope.Payload.FirstName}");
            return Task.CompletedTask;
        }
    }

    private sealed class StubMaterializer : IEnvelopeMaterializer
    {
        public bool WasCalled { get; private set; }

        public TypedEnvelope<TPayload> Materialize<TPayload>(RawEnvelope source, Func<string, TPayload> payloadFactory)
        {
            WasCalled = true;

            object payload = new PersonPayload("Stub", "Stub", "Stub");

            return new TypedEnvelope<TPayload>(
                source.FirstName,
                source.LastName,
                source.City,
                (TPayload)payload);
        }
    }
}