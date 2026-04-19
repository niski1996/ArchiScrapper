using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Messaging.Extensions;
using ArchiScrapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class EnvelopePublicationPolicyBuilderTests
{
    [Fact]
    public void BuilderCanBeResolvedFromDiAndUsedForRetryAndTelemetry()
    {
        var services = new ServiceCollection();
        services.AddCommonMessagingCore();

        var serviceProvider = services.BuildServiceProvider();
        var builder = serviceProvider.GetRequiredService<IEnvelopePublicationPolicyBuilder<PersonPayload>>();

        var telemetry = new RecordingTelemetry();
        var policy = builder
            .UseErrorHandler(new RetryOnceErrorHandler())
            .UseTelemetry(telemetry)
            .Build();

        var publisher = serviceProvider.GetRequiredService<IEnvelopePublisher<PersonPayload>>();
        var source = new TypedEnvelope<PersonPayload>(
            "Jan",
            "Kowalski",
            "Krakow",
            new PersonPayload("Jan", "Kowalski", "Krakow"));

        var attempts = 0;
        var result = publisher.PublishInlineWithPolicy(
            source,
            _ =>
            {
                attempts++;

                if (attempts == 1)
                {
                    throw new InvalidOperationException("Transient failure.");
                }

                return "payload-ok";
            },
            policy);

        Assert.Equal("payload-ok", result.Payload);
        Assert.Equal(["start:Serialize:1", "fail:Serialize:1", "start:Serialize:2", "success:Serialize:2"], telemetry.Events);
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);

    private sealed class RetryOnceErrorHandler : IEnvelopePublicationErrorHandler<PersonPayload>
    {
        private int attempts;

        public Task<EnvelopePublicationErrorDecision> HandleAsync(EnvelopePublicationErrorContext<PersonPayload> context)
        {
            attempts++;

            return Task.FromResult(attempts == 1 ? EnvelopePublicationErrorDecision.Retry : EnvelopePublicationErrorDecision.Stop);
        }
    }

    private sealed class RecordingTelemetry : IEnvelopePublicationTelemetry<PersonPayload>
    {
        public List<string> Events { get; } = [];

        public void OnStepStarting(EnvelopePublicationTelemetryContext<PersonPayload> context)
        {
            Events.Add($"start:{context.StepKind}:{context.Attempt}");
        }

        public void OnStepSucceeded(EnvelopePublicationTelemetryContext<PersonPayload> context)
        {
            Events.Add($"success:{context.StepKind}:{context.Attempt}");
        }

        public void OnStepFailed(EnvelopePublicationTelemetryContext<PersonPayload> context)
        {
            Events.Add($"fail:{context.StepKind}:{context.Attempt}");
        }
    }
}