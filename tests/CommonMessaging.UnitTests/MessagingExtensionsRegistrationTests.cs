using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Extensions;
using ArchiScrapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class MessagingExtensionsRegistrationTests
{
    [Fact]
    public async Task AddCommonMessagingCoreAndFlowRegistersExecutableFlow()
    {
        var services = new ServiceCollection();

        services
            .AddCommonMessagingCore()
            .AddRawEventProcessingFlow<PersonPayload>();

        services.AddSingleton<IEventConsumer<PersonPayload>, SpyConsumer>();

        var serviceProvider = services.BuildServiceProvider();
        var flow = serviceProvider.GetRequiredService<IRawEventProcessingFlow<PersonPayload>>();

        var source = new RawEnvelope(
            "Jan",
            "Kowalski",
            "Krakow",
            JsonSerializer.Serialize(new PersonPayload("Jan", "Kowalski", "Krakow")));

        await flow.ProcessAsync(source, payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);

        var consumer = (SpyConsumer)serviceProvider.GetRequiredService<IEventConsumer<PersonPayload>>();
        Assert.True(consumer.Handled);
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);

    private sealed class SpyConsumer : IEventConsumer<PersonPayload>
    {
        public bool Handled { get; private set; }

        public Task HandleAsync(HandleContext<PersonPayload> context, CancellationToken cancellationToken)
        {
            Handled = true;
            return Task.CompletedTask;
        }
    }
}
