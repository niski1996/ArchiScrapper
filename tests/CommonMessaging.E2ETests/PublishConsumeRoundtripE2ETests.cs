using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Extensions;
using ArchiScrapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArchiScrapper.CommonMessaging.E2ETests;

public class PublishConsumeRoundtripE2ETests
{
    [Fact]
    public async Task InlinePayloadCanBePublishedAndConsumed()
    {
        var services = new ServiceCollection();

        services
            .AddCommonMessagingCore()
            .AddRawEventProcessingFlow<StudentImported>();

        services.AddSingleton<IEventConsumer<StudentImported>, RecordingStudentConsumer>();

        var serviceProvider = services.BuildServiceProvider();
        var publisher = serviceProvider.GetRequiredService<IEnvelopePublisher<StudentImported>>();
        var flow = serviceProvider.GetRequiredService<IRawEventProcessingFlow<StudentImported>>();
        var consumer = (RecordingStudentConsumer)serviceProvider.GetRequiredService<IEventConsumer<StudentImported>>();

        var typed = new TypedEnvelope<StudentImported>(
            "Anna",
            "Nowak",
            "Warsaw",
            new StudentImported("Anna", "Nowak", "Warsaw"));

        var raw = publisher.Publish(typed, payload => JsonSerializer.Serialize(payload));

        await flow.ProcessAsync(raw, DeserializeStudentImported);

        Assert.Equal("Anna", consumer.LastHandled?.FirstName);
        Assert.Equal("Warsaw", consumer.LastHandled?.City);
    }

    [Fact]
    public async Task ReferencedPayloadCanBePublishedAndConsumed()
    {
        var services = new ServiceCollection();

        services
            .AddCommonMessagingCore()
            .AddRawEventProcessingFlow<StudentImported>();

        services.AddSingleton<IEventConsumer<StudentImported>, RecordingStudentConsumer>();

        var serviceProvider = services.BuildServiceProvider();
        var publisher = serviceProvider.GetRequiredService<IEnvelopePublisher<StudentImported>>();
        var flow = serviceProvider.GetRequiredService<IRawEventProcessingFlow<StudentImported>>();
        var consumer = (RecordingStudentConsumer)serviceProvider.GetRequiredService<IEventConsumer<StudentImported>>();

        var typed = new TypedEnvelope<StudentImported>(
            "Jan",
            "Kowalski",
            "Krakow",
            new StudentImported("Jan", "Kowalski", "Krakow"));

        var raw = publisher.PublishWithReference(typed, payload => JsonSerializer.Serialize(payload), "student-ref-1");

        await flow.ProcessAsync(raw, DeserializeStudentImported);

        Assert.Equal("Jan", consumer.LastHandled?.FirstName);
        Assert.Equal("Krakow", consumer.LastHandled?.City);
    }

    private static StudentImported DeserializeStudentImported(string payload)
    {
        return JsonSerializer.Deserialize<StudentImported>(payload)
            ?? throw new InvalidOperationException("Unable to deserialize StudentImported payload.");
    }

    private sealed record StudentImported(string FirstName, string LastName, string City);

    private sealed class RecordingStudentConsumer : IEventConsumer<StudentImported>
    {
        public StudentImported? LastHandled { get; private set; }

        public Task HandleAsync(HandleContext<StudentImported> context, CancellationToken cancellationToken)
        {
            LastHandled = context.Envelope.Payload;
            return Task.CompletedTask;
        }
    }
}