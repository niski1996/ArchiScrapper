using System.Diagnostics;
using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.CommonMessaging.PerformanceTests;

public class ProcessingPerformanceTests
{
    [Fact]
    public async Task LargePayloadProcessingCompletesInReasonableTime()
    {
        var flow = new RawEventProcessingFlowBuilder<PersonPayload>()
            .UseHandler(new NoOpConsumer())
            .Build();

        var largeText = new string('X', 2 * 1024 * 1024);
        var source = new ResolvingExampleEvent(
            "Jan",
            "Kowalski",
            "Krakow",
            JsonSerializer.Serialize(new PersonPayload("Jan", "Kowalski", largeText)));

        var stopwatch = Stopwatch.StartNew();
        await flow.ProcessAsync(source, payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);
        stopwatch.Stop();

        Assert.True(stopwatch.Elapsed.TotalSeconds < 10);
    }

    [Fact]
    public async Task BatchLikeProcessingHandlesMultipleMessages()
    {
        var handledCount = 0;

        var flow = new RawEventProcessingFlowBuilder<PersonPayload>()
            .UseHandler(new CountingConsumer(() => handledCount++))
            .Build();

        for (var index = 0; index < 200; index++)
        {
            var source = new ResolvingExampleEvent(
                "Jan",
                "Kowalski",
                "Krakow",
                JsonSerializer.Serialize(new PersonPayload("Jan", "Kowalski", $"City-{index}")));

            await flow.ProcessAsync(source, payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);
        }

        Assert.Equal(200, handledCount);
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);

    private sealed class NoOpConsumer : IEventConsumer<PersonPayload>
    {
        public Task HandleAsync(HandleContext<PersonPayload> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class CountingConsumer(Action increment) : IEventConsumer<PersonPayload>
    {
        public Task HandleAsync(HandleContext<PersonPayload> context, CancellationToken cancellationToken)
        {
            increment();
            return Task.CompletedTask;
        }
    }
}
