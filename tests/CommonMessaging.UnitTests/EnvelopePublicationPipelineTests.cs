using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class EnvelopePublicationPipelineTests
{
    [Fact]
    public void ComposeCreatesRawEnvelopeWithSerializedPayload()
    {
        var pipeline = new EnvelopePublicationPipeline();
        var source = new TypedEnvelope<PersonPayload>(
            "Jan",
            "Kowalski",
            "Krakow",
            new PersonPayload("Jan", "Kowalski", "Krakow"));

        var result = pipeline.Compose(
            source,
            payload => System.Text.Json.JsonSerializer.Serialize(payload));

        Assert.Equal(source.FirstName, result.FirstName);
        Assert.Equal(source.LastName, result.LastName);
        Assert.Equal(source.City, result.City);
        Assert.Equal("{\"FirstName\":\"Jan\",\"LastName\":\"Kowalski\",\"City\":\"Krakow\"}", result.Payload);
        Assert.Null(result.PayloadReference);
    }

    [Fact]
    public void ComposeThrowsWhenSourceIsNull()
    {
        var pipeline = new EnvelopePublicationPipeline();

        Assert.Throws<ArgumentNullException>(() => pipeline.Compose<PersonPayload>(null!, payload => payload.FirstName));
    }

    [Fact]
    public void ComposeThrowsWhenSerializerIsNull()
    {
        var pipeline = new EnvelopePublicationPipeline();
        var source = new TypedEnvelope<PersonPayload>(
            "Jan",
            "Kowalski",
            "Krakow",
            new PersonPayload("Jan", "Kowalski", "Krakow"));

        Assert.Throws<ArgumentNullException>(() => pipeline.Compose(source, null!));
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);
}