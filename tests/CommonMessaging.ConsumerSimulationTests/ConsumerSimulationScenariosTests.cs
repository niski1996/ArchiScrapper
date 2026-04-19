using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Messaging.Extensions;
using ArchiScrapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArchiScrapper.CommonMessaging.ConsumerSimulationTests;

public class ConsumerSimulationScenariosTests
{
    private static readonly string[] HandlerOnlySequence = ["handler"];
    private static readonly string[] FullPipelineSequence = ["infra", "business", "handler"];
    private static readonly string[] InfraThenHandlerSequence = ["infra", "handler"];
    private static readonly string[] BusinessThenHandlerSequence = ["business", "handler"];

    [Fact]
    public async Task MinimalConfigurationProcessesTypedPayload()
    {
        var sink = new RecordingSink();
        var host = new ConsumerSimulationTestHost<StudentImported>()
            .AddSingleton(sink)
            .AddConsumer<RecordingStudentHandler>();

        var serviceProvider = host.BuildServiceProvider();
        var flow = host.BuildFlow();

        var raw = CreateInlineRawEnvelope("Anna", "Nowak", "Warsaw");
        await flow.ProcessAsync(raw, DeserializeStudentImported);

        Assert.Equal(HandlerOnlySequence, sink.Calls);
        Assert.Equal("Anna", sink.LastHandled?.FirstName);
    }

    [Fact]
    public async Task PayloadReferenceScenarioResolvesPayloadFromStorage()
    {
        var sink = new RecordingSink();
        var host = new ConsumerSimulationTestHost<StudentImported>()
            .AddSingleton(sink)
            .AddConsumer<RecordingStudentHandler>();

        var serviceProvider = host.BuildServiceProvider();
        var storage = (InMemoryPayloadStorageProvider)serviceProvider.GetRequiredService<IPayloadStorageProvider>();
        storage.PutPayload("student-ref-1", JsonSerializer.Serialize(new StudentImported("Jan", "Kowalski", "Krakow")));

        var flow = host.BuildFlow();
        var raw = new RawEnvelope("Jan", "Kowalski", "Krakow", string.Empty, "student-ref-1");

        await flow.ProcessAsync(raw, DeserializeStudentImported);

        Assert.Equal("Jan", sink.LastHandled?.FirstName);
    }

    [Fact]
    public async Task CustomStepsAreExecutedInFrameworkBusinessHandlerOrder()
    {
        var sink = new RecordingSink();
        var host = new ConsumerSimulationTestHost<StudentImported>()
            .AddSingleton(sink)
            .AddInfrastructureStep<FrameworkAuditStep>()
            .AddBusinessStep<ValidateStudentStep>()
            .AddConsumer<RecordingStudentHandler>();

        var flow = host.BuildFlow();

        var raw = CreateInlineRawEnvelope("Anna", "Nowak", "Warsaw");
        await flow.ProcessAsync(raw, DeserializeStudentImported);

        Assert.Equal(FullPipelineSequence, sink.Calls);
        Assert.True(sink.ValidationExecuted);
    }

    [Fact]
    public void MissingConsumerRegistrationFailsFastAtResolutionTime()
    {
        var host = new ConsumerSimulationTestHost<StudentImported>();
        var serviceProvider = host.BuildServiceProvider();

        Assert.Throws<InvalidOperationException>(
            () => serviceProvider.GetRequiredService<IRawEventProcessingFlow<StudentImported>>());
    }

    [Fact]
    public async Task ExceptionInCustomStepIsPropagatedAndHandlerIsNotExecuted()
    {
        var sink = new RecordingSink();
        var host = new ConsumerSimulationTestHost<StudentImported>()
            .AddSingleton(sink)
            .AddInfrastructureStep<ThrowingInfrastructureStep>()
            .AddConsumer<RecordingStudentHandler>();

        var flow = host.BuildFlow();

        var raw = CreateInlineRawEnvelope("Anna", "Nowak", "Warsaw");

        await Assert.ThrowsAsync<InvalidOperationException>(() => flow.ProcessAsync(raw, DeserializeStudentImported));

        Assert.Empty(sink.Calls);
    }

    [Fact]
    public async Task TwoServicesCanUseDifferentPipelineConfigurationIndependently()
    {
        var firstSink = new RecordingSink();
        var secondSink = new RecordingSink();

        var firstHost = new ConsumerSimulationTestHost<StudentImported>()
            .AddSingleton(firstSink)
            .AddInfrastructureStep<FrameworkAuditStep>()
            .AddConsumer<RecordingStudentHandler>();

        var secondHost = new ConsumerSimulationTestHost<StudentImported>()
            .AddSingleton(secondSink)
            .AddBusinessStep<ValidateStudentStep>()
            .AddConsumer<RecordingStudentHandler>();

        var raw = CreateInlineRawEnvelope("Anna", "Nowak", "Warsaw");

        var firstFlow = firstHost.BuildFlow();
        await firstFlow.ProcessAsync(raw, DeserializeStudentImported);

        var secondFlow = secondHost.BuildFlow();
        await secondFlow.ProcessAsync(raw, DeserializeStudentImported);

        Assert.Equal(InfraThenHandlerSequence, firstSink.Calls);
        Assert.Equal(BusinessThenHandlerSequence, secondSink.Calls);
    }

    private static RawEnvelope CreateInlineRawEnvelope(string firstName, string lastName, string city)
    {
        var payload = JsonSerializer.Serialize(new StudentImported(firstName, lastName, city));
        return new RawEnvelope(firstName, lastName, city, payload);
    }

    private static StudentImported DeserializeStudentImported(string payload)
    {
        return JsonSerializer.Deserialize<StudentImported>(payload)
            ?? throw new InvalidOperationException("Unable to deserialize StudentImported payload.");
    }

    private sealed record StudentImported(string FirstName, string LastName, string City);

    private sealed class RecordingSink
    {
        public List<string> Calls { get; } = [];

        public StudentImported? LastHandled { get; set; }

        public bool ValidationExecuted { get; set; }
    }

    private sealed class FrameworkAuditStep(RecordingSink sink) : IInfrastructureStep<StudentImported>
    {
        public async Task ExecuteAsync(HandleContext<StudentImported> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            sink.Calls.Add("infra");
            await continuation(cancellationToken);
        }
    }

    private sealed class ValidateStudentStep(RecordingSink sink) : IBusinessStep<StudentImported>
    {
        public async Task ExecuteAsync(HandleContext<StudentImported> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            sink.ValidationExecuted = true;
            sink.Calls.Add("business");
            await continuation(cancellationToken);
        }
    }

    private sealed class ThrowingInfrastructureStep : IInfrastructureStep<StudentImported>
    {
        public Task ExecuteAsync(HandleContext<StudentImported> context, HandleStepContinuation continuation, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Custom infrastructure step failed.");
        }
    }

    private sealed class RecordingStudentHandler(RecordingSink sink) : IEventConsumer<StudentImported>
    {
        public Task HandleAsync(HandleContext<StudentImported> context, CancellationToken cancellationToken)
        {
            sink.LastHandled = context.Envelope.Payload;
            sink.Calls.Add("handler");
            return Task.CompletedTask;
        }
    }
}
